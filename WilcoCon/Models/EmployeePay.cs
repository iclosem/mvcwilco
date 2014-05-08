using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WilcoCon.Models
{
    public class EmployeePay
    {
        private WilcoEntities db = new WilcoEntities();
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal EmpTotalOTPay { get; set; }//
        public string EmployeeName { get; set; }//
        public string SSN { get; set; }//
        public DateTime Date { get; set; }//
        public DateTime WeekEndDate { get; set; }//
        public int TaxYear { get; set; }//
        public int EEOStatus { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal GrossPay { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalDeductions { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal NetPay { get; set; }//
        public decimal CheckNum { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal WeeklyFedTax { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal WeeklyStateTax { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal WeeklySSTax { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal OtherTax { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal YTDGrossPay { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal YTDFedTax { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal YTDStateTax { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal YTDSSTax { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal YTDTotalDeductions { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal YTDOtherTax { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal YTDNetPay { get; set; }//
        public List<EmployeeRegularWork> EmployeeRegWorkList;//
        public List<EmployeeOvertimeWork> EmployeeOTWorkList;//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double EmpTotalPayHours { get; set; }//
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal EmpTotalRegularPay { get; set; }//
        public double EmpOTTotalPayHours { get; set; }//
        public EmployeePay GetEmployeeReport(Employee employee, DateTime date)
        {
            Repository repository = new Repository();
            EmployeePay empPay = new EmployeePay();
            empPay.EmployeeRegWorkList = new List<EmployeeRegularWork>();
            empPay.EmployeeOTWorkList = new List<EmployeeOvertimeWork>();
            empPay.EmployeeName = employee.FirstName + " " + employee.LastName;
            empPay.SSN = employee.SSN;
            if(date == null)
            {
                empPay.Date = DateTime.Now;
            }
            else {
                empPay.Date = date;
            }
            empPay.WeekEndDate = repository.GetEndDate(date);
            empPay.TaxYear = DateTime.Now.Year;
            empPay.EEOStatus = employee.EEOCOde;
            var timecardsForEmployee = repository.TimeCardsForEmployee(employee, date);
            double hoursWorkedByEmployee = 0;
            foreach (TimeCard timecard in timecardsForEmployee)
            {
                double hoursWorkedOnTimeCard = timecard.TimeOut.Subtract(timecard.TimeIn).TotalHours;
                 if (hoursWorkedByEmployee >= 40)
                 {
                     empPay.AddOTHours(empPay, timecard);
                     hoursWorkedByEmployee += hoursWorkedOnTimeCard;
                 }
                 else if (hoursWorkedByEmployee + hoursWorkedOnTimeCard > 40)
                 {
                     double otHoursWorkedThisShift = 0;
                     if (hoursWorkedOnTimeCard > 8)
                     {
                         otHoursWorkedThisShift = hoursWorkedOnTimeCard - 8;
                     }
                     double regHours = 40 - hoursWorkedByEmployee;
                     double otHours = hoursWorkedOnTimeCard - regHours;
                     if (otHours >= otHoursWorkedThisShift)
                     {
                         empPay.AddRegHours(empPay, timecard, regHours);
                         empPay.AddOTHours(empPay, timecard, otHours);
                     }
                     else
                     {
                         empPay.AddRegHours(empPay, timecard, regHours);
                         empPay.AddOTHours(empPay, timecard, otHoursWorkedThisShift);
                     }
                     hoursWorkedByEmployee += hoursWorkedOnTimeCard;
                 }
                 else
                 {
                     double otHoursWorkedThisShift = 0;
                     if (hoursWorkedOnTimeCard > 8)
                     {
                         otHoursWorkedThisShift = hoursWorkedOnTimeCard - 8;
                     }
                     if (otHoursWorkedThisShift > 0)
                     {
                         empPay.AddRegHours(empPay, timecard, 8);
                         empPay.AddOTHours(empPay, timecard, otHoursWorkedThisShift);
                     }
                     else 
                     {
                         empPay.AddRegHours(empPay, timecard);
                         hoursWorkedByEmployee += hoursWorkedOnTimeCard;
                     }
                 }
            }
            empPay.EmpTotalPayHours = (from r in empPay.EmployeeRegWorkList
                                select r.Hours).Sum();
            empPay.EmpTotalRegularPay = (from r in empPay.EmployeeRegWorkList
                                  select r.Gross).Sum();
            empPay.EmpOTTotalPayHours = (from r in empPay.EmployeeOTWorkList
                                  select r.Hours).Sum();
            empPay.EmpTotalOTPay = (from r in empPay.EmployeeOTWorkList
                             select r.Gross).Sum();
            empPay.GrossPay = empPay.EmpTotalRegularPay + empPay.EmpTotalOTPay;
            Random rand = new Random();
            empPay.CheckNum = rand.Next(1000,10000);
            empPay.WeeklyFedTax = empPay.GrossPay * .15M;
            empPay.WeeklyStateTax = empPay.GrossPay * .04M;
            empPay.WeeklySSTax = empPay.GrossPay * .062M;
            empPay.OtherTax = empPay.GrossPay * .03M;
            empPay.TotalDeductions = empPay.WeeklyFedTax + empPay.WeeklyStateTax + empPay.WeeklySSTax + empPay.OtherTax;
            empPay.NetPay = empPay.GrossPay - empPay.TotalDeductions;
            DateTime begYear = new DateTime(empPay.TaxYear, 1, 1);
            double numWeeks = (empPay.Date - begYear).TotalDays / 7;
            empPay.YTDFedTax = empPay.WeeklyFedTax * (decimal)numWeeks;
            empPay.YTDStateTax = empPay.WeeklyStateTax * (decimal)numWeeks;
            empPay.YTDSSTax = empPay.WeeklySSTax * (decimal)numWeeks;
            empPay.YTDOtherTax = empPay.OtherTax * (decimal)numWeeks;
            empPay.YTDGrossPay = empPay.GrossPay * (decimal)numWeeks;
            empPay.YTDTotalDeductions = empPay.YTDFedTax + empPay.YTDStateTax + empPay.YTDSSTax + empPay.YTDOtherTax;
            empPay.YTDNetPay = empPay.YTDGrossPay - empPay.YTDTotalDeductions;
            return empPay;
        }
        public void AddRegHours(EmployeePay empPay, TimeCard timecard)
        {
            EmployeeRegularWork Work = new EmployeeRegularWork();
            Work.ProjectName = (from p in db.Projects
                                  where p.ProjectId == timecard.ProjectId
                                  select p.ProjectNumber).FirstOrDefault();
            Work.SkillCode = (from j in db.JobClassifications
                                where j.SkillId == timecard.SkillId
                                select j.SkillCode).FirstOrDefault();
            Work.Rate = (from p in db.PayScales
                           where p.SkillId == timecard.SkillId
                           select p.BasicHourlyRate).FirstOrDefault();
            Work.Fringe = (from p in db.PayScales
                             where p.SkillId == timecard.SkillId
                             select p.FringeBenefitPayments).FirstOrDefault();
            Work.Total = Work.Rate + Work.Fringe;
            Work.Hours = timecard.TimeOut.Subtract(timecard.TimeIn).TotalHours;
            Work.Gross = Work.Total * ((decimal)Work.Hours);
            empPay.EmployeeRegWorkList.Add(Work);
            
        }
        public void AddOTHours(EmployeePay empPay, TimeCard timecard)
        {
            EmployeeOvertimeWork otWork = new EmployeeOvertimeWork();
            otWork.ProjectName = (from p in db.Projects
                                  where p.ProjectId == timecard.ProjectId
                                  select p.ProjectNumber).FirstOrDefault();
            otWork.SkillCode = (from j in db.JobClassifications
                                where j.SkillId == timecard.SkillId
                                select j.SkillCode).FirstOrDefault();
            otWork.Rate = (from p in db.PayScales
                           where p.SkillId == timecard.SkillId
                           select p.BasicHourlyRate).FirstOrDefault() * 1.5M;
            otWork.Fringe = (from p in db.PayScales
                             where p.SkillId == timecard.SkillId
                             select p.FringeBenefitPayments).FirstOrDefault();
            otWork.Total = otWork.Rate + otWork.Fringe;
            otWork.Hours = timecard.TimeOut.Subtract(timecard.TimeIn).TotalHours;
            otWork.Gross = otWork.Total * ((decimal)otWork.Hours);
            empPay.EmployeeOTWorkList.Add(otWork);
        }
        public void AddRegHours(EmployeePay empPay, TimeCard timecard, double hoursToAdd)
        {
            EmployeeRegularWork Work = new EmployeeRegularWork();
            Work.ProjectName = (from p in db.Projects
                                where p.ProjectId == timecard.ProjectId
                                select p.ProjectNumber).FirstOrDefault();
            Work.SkillCode = (from j in db.JobClassifications
                              where j.SkillId == timecard.SkillId
                              select j.SkillCode).FirstOrDefault();
            Work.Rate = (from p in db.PayScales
                         where p.SkillId == timecard.SkillId
                         select p.BasicHourlyRate).FirstOrDefault();
            Work.Fringe = (from p in db.PayScales
                           where p.SkillId == timecard.SkillId
                           select p.FringeBenefitPayments).FirstOrDefault();
            Work.Total = Work.Rate + Work.Fringe;
            Work.Hours = hoursToAdd;
            Work.Gross = Work.Total * ((decimal)Work.Hours);
            empPay.EmployeeRegWorkList.Add(Work);
        }
        public void AddOTHours(EmployeePay empPay, TimeCard timecard, double hoursToAdd)
        {
            EmployeeOvertimeWork otWork = new EmployeeOvertimeWork();
            otWork.ProjectName = (from p in db.Projects
                                  where p.ProjectId == timecard.ProjectId
                                  select p.ProjectNumber).FirstOrDefault();
            otWork.SkillCode = (from j in db.JobClassifications
                                where j.SkillId == timecard.SkillId
                                select j.SkillCode).FirstOrDefault();
            otWork.Rate = (from p in db.PayScales
                           where p.SkillId == timecard.SkillId
                           select p.BasicHourlyRate).FirstOrDefault() * 1.5M;
            otWork.Fringe = (from p in db.PayScales
                             where p.SkillId == timecard.SkillId
                             select p.FringeBenefitPayments).FirstOrDefault();
            otWork.Total = otWork.Rate + otWork.Fringe;
            otWork.Hours = hoursToAdd;
            otWork.Gross = otWork.Total * ((decimal)otWork.Hours);
            empPay.EmployeeOTWorkList.Add(otWork);
        }


    }
    public class EmployeeRegularWork
    {
        public string ProjectName { get; set; }
        public string SkillCode { get; set; }
        public decimal Rate { get; set; }
        public decimal Fringe { get; set; }
        public decimal Total { get; set; }
        public double Hours { get; set; }
        public decimal Gross { get; set; }
        
    }
    public class EmployeeOvertimeWork
    {
        public string ProjectName { get; set; }
        public string SkillCode { get; set; }
        public decimal Rate { get; set; }
        public decimal Fringe { get; set; }
        public decimal Total { get; set; }
        public double Hours { get; set; }
        public decimal Gross { get; set; }
    }
}