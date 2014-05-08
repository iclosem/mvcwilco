using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace WilcoCon.Models
{
    public class Repository
    {
        private WilcoEntities db = new WilcoEntities();
        public static string RenderPartialViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, null);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.ToString();
            }
        }
        public IQueryable<Employee> EmployeesInProject(Project project, DateTime currentdate)
        {
           DateTime beginDate = GetBeginDate(currentdate);
           DateTime endDate = GetEndDate(currentdate);
            var employeesInProject = (from t in db.TimeCards
                                       where t.ProjectId == project.ProjectId
                                       where t.Date >= beginDate
                                       where t.Date <= endDate
                                       select t.Employee).Distinct();
            return employeesInProject;
        }
        public IQueryable<TimeCard> TimeCardsInProject(Project project)
        {
            var timecardsInProject = from t in db.TimeCards
                                     where t.ProjectId == project.ProjectId
                                     select t;
            return timecardsInProject;
        }
        public IQueryable<TimeCard> TimeCardsForEmployee(Employee employee, DateTime date)
        {
            DateTime begDate = GetBeginDate(date);
            DateTime endDate = GetEndDate(date);
            var timecardsForEmployee = from t in db.TimeCards
                                       where t.Employee.EmployeeId == employee.EmployeeId
                                       where t.Date >= begDate
                                       where t.Date <= endDate
                                       select t;
            return timecardsForEmployee;
        }
        public IQueryable<JobClassification> AllJobClassifications()
        {
            var allJobClassifications = from j in db.JobClassifications
                                        select j;
            return allJobClassifications;
        }
        public TimeSpan HoursWorked(Project project)
        {
            var employeesInProject = from t in db.TimeCards
                                     where t.ProjectId == project.ProjectId
                                     select t.Employee;
            TimeSpan sumHours = TimeSpan.FromHours(0);
            foreach (Employee e in employeesInProject)
            {
                TimeSpan hoursWorkedOnTimeCard;
                var timeCardsForEmployee = from t in db.TimeCards
                                           where t.EmployeeId == e.EmployeeId
                                           select t;
                foreach (TimeCard timecard in timeCardsForEmployee)
                {
                    hoursWorkedOnTimeCard = timecard.TimeOut - timecard.TimeIn;
                    sumHours = hoursWorkedOnTimeCard + sumHours;
                }

            }
            return sumHours;

        }
        public TimeSpan HoursWorked(Project project, string skillcode, DateTime currentdate)
        {
            DateTime endDate = GetEndDate(currentdate);
            DateTime beginDate = GetBeginDate(currentdate);
            var query = from t in db.TimeCards
                        where t.ProjectId == project.ProjectId
                        where t.Date >= beginDate
                        where t.Date <= endDate
                        select t;
            TimeSpan sumHours = TimeSpan.Zero;
            foreach (TimeCard t in query)
            {
                if (t.JobClassification.SkillCode == skillcode)
                {
                    sumHours = sumHours.Add(t.TimeOut.Subtract(t.TimeIn));
                }
            }
            return sumHours;
        }
 
        public TimeSpan HoursWorked(Project project, string gender, int eeoccode,string skillcode, DateTime endDate)
        {
            DateTime beginDate = GetBeginDate(endDate);
            var timeCardsInProject = from t in db.TimeCards
                                     where t.ProjectId == project.ProjectId
                                     where t.JobClassification.SkillCode == skillcode
                                     where t.Date >=beginDate
                                     where t.Date <= endDate
                                     select t;
            var numOfTimeCards = timeCardsInProject.Count();
            TimeSpan sumHours = TimeSpan.Zero;
            foreach (TimeCard t in timeCardsInProject)
            {
                var employeesInProject = from e in db.Employees
                                         where t.EmployeeId == e.EmployeeId
                                         where e.Gender == gender
                                         where e.EEOCOde == eeoccode
                                         select e;
                foreach (Employee e in employeesInProject)
                {
                    if (e.EmployeeId == t.EmployeeId)
                    {
                        sumHours = sumHours.Add(t.TimeOut.Subtract(t.TimeIn));
                    }
                }
            }
            return sumHours;
        }

        public string GetDate(Project project)
        {
            DateTime startTime = new DateTime(2012, 09, 01);
            DateTime now = DateTime.Now;
            string EndDate;
            var diff = now.Subtract(startTime);
            int daysToEndPeriod = diff.Days % 7;
            if (daysToEndPeriod == 0)
                EndDate = DateTime.Now.ToShortDateString();
            else
                EndDate = (DateTime.Now.AddDays(7 - daysToEndPeriod).Date).ToShortDateString();
            return EndDate;
        }
        public DateTime GetBeginDate(DateTime currentdate) //project is completely redundant and used as a placeholder
        {
            DateTime startTime = new DateTime(2012, 09, 02);
            DateTime now = currentdate;
            DateTime EndDate;
            var diff = now.Subtract(startTime);
            int daysToEndPeriod = diff.Days % 7;
            if (daysToEndPeriod == 0)
                EndDate = currentdate;
            else
                EndDate = (currentdate.AddDays(0-daysToEndPeriod).Date);
            return EndDate;
        }
        public DateTime GetEndDate(DateTime currentdate)
        {
            DateTime startTime = new DateTime(2012, 09, 01);
            DateTime now = currentdate;
            DateTime EndDate;
            var diff = now.Subtract(startTime);
            int daysToEndPeriod = diff.Days % 7;
            if (daysToEndPeriod == 0)
                EndDate = currentdate;
            else
                EndDate = (currentdate.AddDays(7 - daysToEndPeriod).Date);
            return EndDate;
        }
        public string SkillIdToSkillCode(int skillId)
        {
            string skillCodes = (from s in db.JobClassifications
                               where s.SkillId == skillId
                               select s.SkillCode).FirstOrDefault();            
            return skillCodes;
        }
        public decimal GetRate(TimeCard timecard)
        {
            decimal Rate = (from ps in db.PayScales
                       where ps.ProjectId == timecard.ProjectId
                       where ps.SkillId == timecard.SkillId
                       select ps.BasicHourlyRate).FirstOrDefault();
            return Rate;
        }
        public decimal GetFringe(TimeCard timecard)
        {
            decimal Fringe = (from ps in db.PayScales
                              where ps.ProjectId == timecard.ProjectId
                              where ps.SkillId == timecard.SkillId
                              select ps.FringeBenefitPayments).FirstOrDefault();
            return Fringe;
        }
        public List<string> WeekEndDates()
        {
            DateTime startTime = new DateTime(2012, 09, 01);
            DateTime now = DateTime.Now;
            DateTime EndDate;
            List<string> endDateList = new List<string>();
            var diff = now.Subtract(startTime);
            int daysToEndPeriod = diff.Days % 7;
            for (int i = 0; i < 366; i = i + 7)
            {
                if (daysToEndPeriod == 0)
                    EndDate = DateTime.Now.Subtract(TimeSpan.FromDays(i));
                else
                    EndDate = (DateTime.Now.AddDays(7 - daysToEndPeriod).Date).Subtract(TimeSpan.FromDays(i));
                endDateList.Add(EndDate.ToShortDateString());
            }
            return endDateList;
            
        }
        public DateTime CurrentWeekEndDate()
        {
            DateTime startTime = new DateTime(2012, 09, 01);
            DateTime now = DateTime.Now;
            DateTime EndDate;
            var diff = now.Subtract(startTime);
            int daysToEndPeriod = diff.Days % 7;
            if (daysToEndPeriod == 0)
                EndDate = DateTime.Now;
            else
                EndDate = (DateTime.Now.AddDays(7 - daysToEndPeriod).Date);
            return EndDate;
        }

        
    }
}