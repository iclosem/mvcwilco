using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.ComponentModel.DataAnnotations;
namespace WilcoCon.Models
{
    public class ProjectController : Controller
    {
        private WilcoEntities db = new WilcoEntities();

        // GET: /Project/
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }

        //public ActionResult Email()
        //{
        //    dynamic email = new Email("EEOCReport");
        //    email.To = "webninja@example.com";
        //    email.Arbitrary = "random nonsense";
        //    email.Send();
        //    return View();
        //}
        // GET: /Project/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: /Project/Create
        public ActionResult Create()
        {
            IEnumerable<SelectListItem> customers = from c in db.Customers.ToList()
                            select new SelectListItem 
                            { 
                            Text = c.CustomerId.ToString() + "-" + c.LastName + "-" + c.CompanyName,
                            Value = c.CustomerId.ToString()
                            };
            //List<SelectListItem> Customer = new List<SelectListItem>();
            //foreach (Customer c in customers)
            //{
            //    Customer.Add(new SelectListItem { Text = c.CustomerId + "-" + c.LastName + "-" + c.CompanyName, Value=c.CustomerId });
            //}
            //ViewBag.Customer = new SelectList(customers, "Value", "Text");
            //ViewBag.test = new SelectList(Customer, "CustoasdfadfmerId", "test");
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "CustomerId");
            return View();
        }

        // POST: /Project/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ProjectId,ProjectNumber,ProjectLocation,ProjectDescription,DistanceFromUnionHall,ProjectSupervisor,Active,CustomerId")] Project project)
        {
            int countExistingProjects = (from t in db.Projects
                                         where t.ProjectNumber == project.ProjectNumber
                                         select t).Count();
            if (countExistingProjects > 0)
            {
                ModelState.AddModelError("", "Project Number already exists in system, please enter a unique project number");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Projects.Add(project);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    //db.SaveChanges();
                    ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "CustomerId");
                    return RedirectToAction("Index");
                }
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "CustomerId");
            return View(project);
        }

        // GET: /Project/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "CustomerId", project.CustomerId);
            return View(project);
        }

        // POST: /Project/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectId,ProjectNumber,ProjectLocation,ProjectDescription,DistanceFromUnionHall,ProjectSupervisor,CustomerId,Active")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }
        public ActionResult ActiveProjects()
        {
            var activeProjects = from p in db.Projects
                                 where p.Active == true
                                 select p;
            ViewBag.ActiveProjects = activeProjects;
            return View(ViewBag.ActiveProjects);
        }
        public ActionResult EEOCReport(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            Repository repository = new Repository();
            ViewBag.Date = repository.CurrentWeekEndDate().ToShortDateString();
            ViewBag.ProjectLocation = project.ProjectLocation;
            ViewBag.ProjectDescription = project.ProjectDescription;
            ViewBag.WeekEndDates = repository.WeekEndDates();
            ViewBag.ProjectNumber = project.ProjectNumber;
            
            EEOCTimeCard eeoctimecards = new EEOCTimeCard();
            var eeoctimecardList = eeoctimecards.AllHoursWorked(project,repository.GetEndDate(DateTime.Now));
            ViewBag.TotalMaleMinorityHours = (from et in eeoctimecardList
                                             select et.MaleMinorityHours).Sum();
            ViewBag.TotalFemaleMinorityHours = (from et in eeoctimecardList
                                                select et.FemaleMinorityHours).Sum();
            ViewBag.TotalMaleNonMinorityHours = (from et in eeoctimecardList
                                                select et.MaleNonMinorityHours).Sum();
            ViewBag.TotalFemaleNonMinorityHours = (from et in eeoctimecardList
                                                select et.FemaleNonMinorityHours).Sum();
            ViewBag.TotalJobHours = (from et in eeoctimecardList
                                                select et.JobHours).Sum();
            double totalHoursWorkedByMaleMinority = (from et in eeoctimecardList
                                              select et.MaleMinorityHours).Sum();
            double totalHoursWorkedByFemaleMinority = (from et in eeoctimecardList
                                                    select et.FemaleMinorityHours).Sum();
            double totalHoursWorkedByFemaleNonMinority = (from et in eeoctimecardList
                                                       select et.FemaleNonMinorityHours).Sum();
            ViewBag.TotalPercentageMinority = ((totalHoursWorkedByMaleMinority + totalHoursWorkedByFemaleMinority) / ViewBag.TotalJobHours).ToString("P");
            ViewBag.TotalPercentageFemale = ((totalHoursWorkedByFemaleMinority + totalHoursWorkedByFemaleNonMinority) / ViewBag.TotalJobHours).ToString("P");
            return View(eeoctimecardList);
        }
        [HttpPost]
        public ActionResult EEOCReport(int? id, DateTime WeekEndDate)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            Repository repository = new Repository();
            ViewBag.Date = WeekEndDate.ToShortDateString();
            ViewBag.ProjectLocation = project.ProjectLocation;
            ViewBag.ProjectDescription = project.ProjectDescription;
            ViewBag.ProjectNumber = project.ProjectNumber;
            ViewBag.WeekEndDates = repository.WeekEndDates();
            EEOCTimeCard eeoctimecards = new EEOCTimeCard();
            var eeoctimecardList = eeoctimecards.AllHoursWorked(project, repository.GetEndDate(WeekEndDate));
            ViewBag.TotalMaleMinorityHours = (from et in eeoctimecardList
                                              select et.MaleMinorityHours).Sum();
            ViewBag.TotalFemaleMinorityHours = (from et in eeoctimecardList
                                                select et.FemaleMinorityHours).Sum();
            ViewBag.TotalMaleNonMinorityHours = (from et in eeoctimecardList
                                                 select et.MaleNonMinorityHours).Sum();
            ViewBag.TotalFemaleNonMinorityHours = (from et in eeoctimecardList
                                                   select et.FemaleNonMinorityHours).Sum();
            ViewBag.TotalJobHours = (from et in eeoctimecardList
                                     select et.JobHours).Sum();
            double totalHoursWorkedByMaleMinority = (from et in eeoctimecardList
                                                     select et.MaleMinorityHours).Sum();
            double totalHoursWorkedByFemaleMinority = (from et in eeoctimecardList
                                                       select et.FemaleMinorityHours).Sum();
            double totalHoursWorkedByFemaleNonMinority = (from et in eeoctimecardList
                                                          select et.FemaleNonMinorityHours).Sum();
            double TotalPercentageMinority = ((totalHoursWorkedByMaleMinority + totalHoursWorkedByFemaleMinority) / ViewBag.TotalJobHours);
            ViewBag.TotalPercentageMinority = string.Format("{0:P2}", TotalPercentageMinority);
            ViewBag.TotalPercentageFemale = ((totalHoursWorkedByFemaleMinority + totalHoursWorkedByFemaleNonMinority) / ViewBag.TotalJobHours).ToString("P");
            return View(eeoctimecardList);
        }
        public ActionResult CompReport(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            Repository repository = new Repository();
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.Date = repository.CurrentWeekEndDate().ToShortDateString();
            ViewBag.WeekEndDates = repository.WeekEndDates();
            ViewBag.ProjectLocation = project.ProjectLocation;
            ViewBag.ProjectDescription = project.ProjectDescription;
            ViewBag.ProjectNumber = project.ProjectNumber;
            PayrollReport payrollreports = new PayrollReport();
            return View(payrollreports.PayRollReports(project, repository.CurrentWeekEndDate()));
        }
        [HttpPost]
        public ActionResult CompReport(int? id, DateTime WeekEndDate)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            Repository repository = new Repository();
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.Date = WeekEndDate.ToShortDateString();
            ViewBag.WeekEndDates = repository.WeekEndDates();
            ViewBag.ProjectLocation = project.ProjectLocation;
            ViewBag.ProjectDescription = project.ProjectDescription;
            ViewBag.ProjectNumber = project.ProjectNumber;
            PayrollReport payrollreports = new PayrollReport();

            return View(payrollreports.PayRollReports(project, WeekEndDate));
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: /Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
