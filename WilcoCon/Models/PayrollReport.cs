using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WilcoCon.Models
{
    public class PayrollReport
    {
        public int EmployeeId;
        public string SkillCode;
        public decimal Rate;
        public decimal Fringe;
        public decimal Total;
        public double Hours;
        public decimal Gross;
        public double OTEmployeeNumber;
        public double OTSkillCode;
        public decimal OTRate;
        public decimal OTFringe;
        public decimal OTTotal;
        public double OTHours;
        public decimal OTGross;
        public WilcoEntities db = new WilcoEntities();
        public List<PayrollReport> PayRollReports(Project project)
        {
            Repository repository = new Repository();
            var EmployeesInProject = repository.EmployeesInProject(project, DateTime.Now);
            DateTime beginDate = repository.GetBeginDate(DateTime.Now);
            DateTime endDate = repository.GetEndDate(DateTime.Now);
            var TimeCardsForProject = (from t in db.TimeCards
                                       where t.Date >= beginDate
                                       where t.Date <= endDate
                                       where t.ProjectId == project.ProjectId
                                       orderby t.Date descending
                                       select t).Distinct();
            List<PayrollReport> payrollreportList = new List<PayrollReport>();
            foreach (Employee e in EmployeesInProject)
            {
                var timecardsforemployee = (from t in TimeCardsForProject
                                           where t.EmployeeId == e.EmployeeId
                                           orderby t.Date descending
                                           select t).Distinct();
                
                double totalHoursForEmployee = 0;
                foreach (TimeCard t in timecardsforemployee)
                {
                    PayrollReport payrollreport = new PayrollReport();
                    payrollreport.EmployeeId = t.EmployeeId;
                    payrollreport.SkillCode = repository.SkillIdToSkillCode(t.SkillId);
                    payrollreport.Rate = repository.GetRate(t);
                    payrollreport.Fringe = repository.GetFringe(t);
                    payrollreport.Total = payrollreport.Rate + payrollreport.Fringe;
                    payrollreport.OTRate = payrollreport.Rate * 1.5M;
                    payrollreport.OTTotal = payrollreport.OTRate + payrollreport.Fringe;
                    double totalLengthOfShift = (t.TimeOut - t.TimeIn).TotalMinutes / 60;
                    PayrollReport payrollReportForEmployee = (from p in payrollreportList.ToList()
                                                              where p.EmployeeId == t.EmployeeId
                                                              where p.SkillCode == repository.SkillIdToSkillCode(t.SkillId)
                                                              select p).FirstOrDefault();
                    if (totalHoursForEmployee > 40 )
                    {
                        if (payrollReportForEmployee == null)
                        {
                           payrollreport.OTHours += totalLengthOfShift;
                           payrollreportList.Add(payrollreport);
                        }
                        else
                        {
                            payrollReportForEmployee.OTHours += totalLengthOfShift;
                        }
                    }
                    else if ((totalHoursForEmployee + totalLengthOfShift) > 40)
                    {
                        double otHoursWorkedThisShift = 0;
                        if (totalLengthOfShift > 8)
                        {
                            otHoursWorkedThisShift = totalLengthOfShift - 8;
                        }
                        double regHours = 40 - totalHoursForEmployee;
                        double otHours = totalHoursForEmployee - regHours;
                        if (otHours > otHoursWorkedThisShift)
                        {
                            if (payrollReportForEmployee == null)
                            {
                                payrollreport.Hours += regHours;
                                payrollreport.OTHours += otHours;
                                payrollreportList.Add(payrollreport);
                            }
                            else
                            {
                                payrollReportForEmployee.Hours += regHours;
                                payrollReportForEmployee.OTHours += otHours;
                            }
                        }
                        else
                        {
                            if (payrollReportForEmployee == null)
                            {
                                payrollreport.Hours += regHours;
                                payrollreport.OTHours += totalLengthOfShift - otHoursWorkedThisShift;
                                payrollreportList.Add(payrollreport);
                            }
                            else
                            {
                                payrollReportForEmployee.Hours += regHours;
                                payrollReportForEmployee.OTHours += otHoursWorkedThisShift;
                            }
                        }
                    }
                    else
                    {
                        double otHoursWorkedThisShift = 0;
                        if (totalLengthOfShift > 8)
                        {
                            otHoursWorkedThisShift = totalLengthOfShift - 8;
                        }
                        if (otHoursWorkedThisShift > 0)
                        {
                            double regHours = totalLengthOfShift - otHoursWorkedThisShift;
                            if (payrollReportForEmployee == null)
                            {
                                payrollreport.Hours += regHours;
                                payrollreport.OTHours += totalLengthOfShift - otHoursWorkedThisShift;
                                payrollreportList.Add(payrollreport);
                            }
                            else
                            {
                                payrollReportForEmployee.Hours += regHours;
                                payrollReportForEmployee.OTHours += otHoursWorkedThisShift;
                            }
                        }
                        else
                        {
                            if (payrollReportForEmployee == null)
                            {
                                payrollreport.Hours += totalLengthOfShift;
                                payrollreportList.Add(payrollreport);
                            }
                            else
                            {
                                payrollReportForEmployee.Hours += totalLengthOfShift;
                            }
                        }
                    }
                    if (payrollreportList.Count == 0)
                    {
                        double otHoursWorkedThisShift = 0;
                        if (totalLengthOfShift > 8)
                        {
                            otHoursWorkedThisShift = totalLengthOfShift - 8;
                        }
                        if (otHoursWorkedThisShift > 0)
                        {
                            double regHours = totalLengthOfShift - otHoursWorkedThisShift;
                            payrollreport.Hours += regHours;
                            payrollreport.OTHours += otHoursWorkedThisShift;
                            payrollreportList.Add(payrollreport);
                        }
                        else
                        {
                            payrollreport.Hours += totalLengthOfShift;
                            payrollreportList.Add(payrollreport);
                        }
                    }
                    totalHoursForEmployee += totalLengthOfShift;
                }
            }
            //if two timecards are on the same day then you have to figure out which hours are overtime and which are not.
            //  
            foreach (PayrollReport p in payrollreportList)
            {
                p.Gross = p.Total * (decimal)p.Hours;
                p.OTGross = p.OTTotal * (decimal)p.OTHours;
            }
            return payrollreportList;
        }
        public List<PayrollReport> PayRollReports(Project project, DateTime currentdate)
        {
            Repository repository = new Repository();
            var EmployeesInProject = repository.EmployeesInProject(project, currentdate);
            DateTime beginDate = repository.GetBeginDate(currentdate);
            DateTime endDate = repository.GetEndDate(currentdate);
            var TimeCardsForProject = (from t in db.TimeCards
                                       where t.Date >= beginDate
                                       where t.Date <= endDate
                                       where t.ProjectId == project.ProjectId
                                       orderby t.Date descending
                                       select t).Distinct();
            List<PayrollReport> payrollreportList = new List<PayrollReport>();
            foreach (Employee e in EmployeesInProject)
            {
                var timecardsforemployee = (from t in TimeCardsForProject
                                            where t.EmployeeId == e.EmployeeId
                                            orderby t.Date descending
                                            select t).Distinct();

                double totalHoursForEmployee = 0;
                foreach (TimeCard t in timecardsforemployee)
                {
                    PayrollReport payrollreport = new PayrollReport();
                    payrollreport.EmployeeId = t.EmployeeId;
                    payrollreport.SkillCode = repository.SkillIdToSkillCode(t.SkillId);
                    payrollreport.Rate = repository.GetRate(t);
                    payrollreport.Fringe = repository.GetFringe(t);
                    payrollreport.Total = payrollreport.Rate + payrollreport.Fringe;
                    payrollreport.OTRate = payrollreport.Rate * 1.5M;
                    payrollreport.OTTotal = payrollreport.OTRate + payrollreport.Fringe;
                    double totalLengthOfShift = (t.TimeOut - t.TimeIn).TotalMinutes / 60;
                    PayrollReport payrollReportForEmployee = (from p in payrollreportList.ToList()
                                                              where p.EmployeeId == t.EmployeeId
                                                              where p.SkillCode == repository.SkillIdToSkillCode(t.SkillId)
                                                              select p).FirstOrDefault();
                    if (totalHoursForEmployee > 40)
                    {
                        if (payrollReportForEmployee == null)
                        {
                            payrollreport.OTHours += totalLengthOfShift;
                            payrollreportList.Add(payrollreport);
                        }
                        else
                        {
                            payrollReportForEmployee.OTHours += totalLengthOfShift;
                        }
                    }
                    else if ((totalHoursForEmployee + totalLengthOfShift) > 40)
                    {
                        double otHoursWorkedThisShift = 0;
                        if (totalLengthOfShift > 8)
                        {
                            otHoursWorkedThisShift = totalLengthOfShift - 8;
                        }
                        double regHours = 40 - totalHoursForEmployee;
                        double otHours = totalHoursForEmployee - regHours;
                        if (otHours > otHoursWorkedThisShift)
                        {
                            if (payrollReportForEmployee == null)
                            {
                                payrollreport.Hours += regHours;
                                payrollreport.OTHours += otHours;
                                payrollreportList.Add(payrollreport);
                            }
                            else
                            {
                                payrollReportForEmployee.Hours += regHours;
                                payrollReportForEmployee.OTHours += otHours;
                            }
                        }
                        else
                        {
                            if (payrollReportForEmployee == null)
                            {
                                payrollreport.Hours += regHours;
                                payrollreport.OTHours += totalLengthOfShift - otHoursWorkedThisShift;
                                payrollreportList.Add(payrollreport);
                            }
                            else
                            {
                                payrollReportForEmployee.Hours += regHours;
                                payrollReportForEmployee.OTHours += otHoursWorkedThisShift;
                            }
                        }
                    }
                    else
                    {
                        double otHoursWorkedThisShift = 0;
                        if (totalLengthOfShift > 8)
                        {
                            otHoursWorkedThisShift = totalLengthOfShift - 8;
                        }
                        if (otHoursWorkedThisShift > 0)
                        {
                            double regHours = totalLengthOfShift - otHoursWorkedThisShift;
                            if (payrollReportForEmployee == null)
                            {
                                payrollreport.Hours += regHours;
                                payrollreport.OTHours += totalLengthOfShift - otHoursWorkedThisShift;
                                payrollreportList.Add(payrollreport);
                            }
                            else
                            {
                                payrollReportForEmployee.Hours += regHours;
                                payrollReportForEmployee.OTHours += otHoursWorkedThisShift;
                            }
                        }
                        else
                        {
                            if (payrollReportForEmployee == null)
                            {
                                payrollreport.Hours += totalLengthOfShift;
                                payrollreportList.Add(payrollreport);
                            }
                            else
                            {
                                payrollReportForEmployee.Hours += totalLengthOfShift;
                            }
                        }
                    }
                    if (payrollreportList.Count == 0)
                    {
                        double otHoursWorkedThisShift = 0;
                        if (totalLengthOfShift > 8)
                        {
                            otHoursWorkedThisShift = totalLengthOfShift - 8;
                        }
                        if (otHoursWorkedThisShift > 0)
                        {
                            double regHours = totalLengthOfShift - otHoursWorkedThisShift;
                            payrollreport.Hours += regHours;
                            payrollreport.OTHours += otHoursWorkedThisShift;
                            payrollreportList.Add(payrollreport);
                        }
                        else
                        {
                            payrollreport.Hours += totalLengthOfShift;
                            payrollreportList.Add(payrollreport);
                        }
                    }
                    totalHoursForEmployee += totalLengthOfShift;
                }
            }
            //if two timecards are on the same day then you have to figure out which hours are overtime and which are not.
            //  
            foreach (PayrollReport p in payrollreportList)
            {
                p.Gross = p.Total * (decimal)p.Hours;
                p.OTGross = p.OTTotal * (decimal)p.OTHours;
            }
            return payrollreportList;
        }
    }

}