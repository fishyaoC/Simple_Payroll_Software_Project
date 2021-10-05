using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Simple_Payroll_Software_Project
{
    class Program
    {
        public static void Main(string[] args)
        {
            List<Staff> staffs = new List<Staff>();
            FileReader fileReader = new FileReader();
            int month = 0, year = 0;

            while (year == 0)
            {
                Console.WriteLine("請輸入年份：");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "請再輸入一次");
                }
            }

            while (month == 0)
            {
                Console.WriteLine("請輸入月份：");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());

                    if (month < 1 || month > 12)
                    {
                        Console.WriteLine("請重新輸入有效的月份(1~12)");
                        month = 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "請再輸入一次");
                }
            }

            staffs = fileReader.ReadFile();

            for (int i = 0; i < staffs.Count; i++)
            {
                try
                {
                    Console.WriteLine("請輸入 {0} 的總工時 ", staffs[i].NameOfStaff);
                    staffs[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    staffs[i].CalculatingPay();
                    Console.WriteLine(staffs[i]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(staffs);
            ps.GenerateSummary(staffs);

            Console.Read();
        }

    }

    class Staff
    {
        private float hourlyRate;
        private int hWorked;

        public float TotalPay { get; protected set; }
        public float BasicPay { get; private set; }
        public String NameOfStaff { get; private set; }

        public int HoursWorked
        {
            get
            {
                return hWorked;
            }
            set
            {
                if (value > 0)
                    hWorked = value;
                else
                {
                    hWorked = 0;
                }
            }
        }

        public Staff(String name, float rate)
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        public virtual void CalculatingPay()
        {
            Console.WriteLine("薪資計算中...");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }

        public override string ToString()
        {
            return "\n員工姓名： " + NameOfStaff +
                "\n時薪： " + hourlyRate +
                "\n總工時： " + hWorked +
                "\n基本薪資： " + BasicPay +
                "\n總薪資： " + TotalPay;
        }
    }
    class Manager : Staff 
    {
        private const float managerHourlyRate = 50;
        public int Allowance { get; private set; }
        public Manager(string name) : base(name, managerHourlyRate)
        {
        }

        public override void CalculatingPay()
        {
            base.CalculatingPay();

            if (HoursWorked > 160)
            {
                Allowance = 1000;
                TotalPay = BasicPay + Allowance;
            }
        }
        public override string ToString()
        {
            return "\n員工姓名： " + NameOfStaff +
                "\n經理時薪： " + managerHourlyRate +
                "\n總工時： " + HoursWorked +
                "\n基本薪資： " + BasicPay +
                "\n津貼： " + Allowance+
                "\n總薪資： " + TotalPay;
        }
    }

    class Admin : Staff
    {
        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30f;
        public float Overtime { get; private set; }
        public Admin(string name) : base(name, adminHourlyRate) 
        {
        }

        public override void CalculatingPay()
        {
            base.CalculatingPay();
            if (HoursWorked > 160)
            {
                Overtime = overtimeRate * (HoursWorked - 160);
                TotalPay = overtimeRate + BasicPay;
            }
        }
        public override string ToString()
        {
            return "\n員工姓名： " + NameOfStaff +
                "\n管理員時薪： " + adminHourlyRate +
                "\n總工時： " + HoursWorked +
                "\n基本薪資： " + BasicPay +
                "\n津貼： " + overtimeRate +
                "\n總薪資： " + TotalPay;
        }

    }

    class FileReader
    {
        public List<Staff> ReadFile()
        {
            List<Staff> staffs = new List<Staff>();
            string path = "C:/Users/eric/Desktop/staff.txt";
            string[] result = new string[2];
            string[] separator = { "," };

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);

                        if (result[1] == "Manager")
                        {
                            staffs.Add(new Manager(result[0]));
                        }
                        else if (result[1] == "Admin") //Adam,Admin
                            staffs.Add(new Admin(result[0]));
                    }
                    sr.Close();
                }
            }
            else
            {
                Console.WriteLine("Error: 檔案不存在");
            }

            return staffs;
        }
    }

    class PaySlip
    {
        private int month;
        private int year;

        enum MonthsOfYear
        {
            JAN = 1, FEB = 2, MAR = 3, APR = 4,
            MAY = 5, JUN = 6, JUL = 7, AUG = 8,
            SEP = 9, OCT = 10, NOV = 11, DEC = 12
        }

        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        public void GeneratePaySlip(List<Staff> staffs)
        {
            string path;

            foreach (Staff s in staffs)
            {
                path = s.NameOfStaff + ".txt";//Yvonne.txt

                using (StreamWriter sw = new StreamWriter(path))
                {
                    /*
                        薪資單 DEC 2010
                        =======================
                        員工姓名： Yvonne
                        總工時： 1231
 
                        基本薪資： $61,550
                        津貼： $1,000
 
                        =======================
                        總薪資： $62,550
                        =======================
                    */
                    sw.WriteLine("工資單： {0} {1}", (MonthsOfYear)month, year);
                    sw.WriteLine("=======================");
                    sw.WriteLine("員工： {0}", s.NameOfStaff);
                    sw.WriteLine("總工時： {0}", s.HoursWorked);
                    sw.WriteLine("");
                    sw.WriteLine("基本薪資： {0:C}", s.BasicPay);
                    if (s.GetType() == typeof(Manager))
                        sw.WriteLine("津貼： {0:C}", ((Manager)s).Allowance);
                    else if (s.GetType() == typeof(Admin))
                        sw.WriteLine("加班： {0:C}", ((Admin)s).Overtime);
                    sw.WriteLine("");
                    sw.WriteLine("=======================");
                    sw.WriteLine("總薪資： {0:C}", s.TotalPay);
                    sw.WriteLine("=======================");

                    sw.Close();
                }
            }
        }

        public void GenerateSummary(List<Staff> staffs)
        {
            var result =
                from s in staffs
                where s.HoursWorked < 10
                orderby s.NameOfStaff ascending
                select new { s.NameOfStaff, s.HoursWorked };

            string path = "summary.txt";

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("工作時數少於10小時的員工");
                sw.WriteLine("");

                foreach (var r in result)
                {
                    sw.WriteLine("員工姓名： {0}, 總工時 {1}",
                                 r.NameOfStaff, r.HoursWorked);
                }

                sw.Close();
            }
        }

        public override string ToString()
        {
            return "month = " + month + " year = " + year;
        }
    }

}
