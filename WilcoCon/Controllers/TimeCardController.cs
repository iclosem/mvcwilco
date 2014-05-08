using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace WilcoCon.Models
{
    public class TimeCardController : Controller
    {
        private WilcoEntities db = new WilcoEntities();

        // GET: /TimeCard/
        [Authorize]
        public ActionResult Index(string sortOrder)
        {
            ViewBag.NameSortParm = sortOrder == "Last Name" ? "date_desc" : "name_desc";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.ProjectNumParm = sortOrder == "Project Number" ? "projectnum_desc" : "";
            var timecards = db.TimeCards.Include(t => t.Employee).Include(t => t.JobClassification).Include(t => t.Project);
            switch (sortOrder)
            {
                case "name_desc":
                    timecards = timecards.OrderByDescending(t => t.Employee.LastName);
                    break;
                case "Date":
                    timecards = timecards.OrderBy(t => t.Date);
                    break;
                case "date_desc":
                    timecards = timecards.OrderByDescending(t => t.Date);
                    break;
                case "projectnum_desc":
                    timecards = timecards.OrderByDescending(t => t.Project.ProjectNumber);
                    break;
                default:
                    timecards = timecards.OrderBy(t => t.Date);
                    break;
            }
            return View(timecards.ToList());
        }

        // GET: /TimeCard/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeCard timecard = db.TimeCards.Find(id);
            if (timecard == null)
            {
                return HttpNotFound();
            }
            return View(timecard);
        }

        // GET: /TimeCard/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeId");
            ViewBag.SkillId = new SelectList(db.JobClassifications, "SkillId", "SkillCode");
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "ProjectNumber");
            return View();
        }

        // POST: /TimeCard/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="TimeCardId,EmployeeId,SkillId,ProjectId,Date,TimeIn,TimeOut")] TimeCard timecard)
        {
            int countExistingTimeCards = (from t in db.TimeCards
                                          where t.Date == timecard.Date
                                          where t.TimeOut >= timecard.TimeIn
                                          select t).Count();
            if (countExistingTimeCards > 0)
            {
                ModelState.AddModelError("", "You cannot have overlapping timecards.  Quit trying to steal money.");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.TimeCards.Add(timecard);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeId", timecard.EmployeeId);
            ViewBag.SkillId = new SelectList(db.JobClassifications, "SkillId", "SkillCode", timecard.SkillId);
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "ProjectNumber", timecard.ProjectId);
            return View(timecard);
        }

        // GET: /TimeCard/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeCard timecard = db.TimeCards.Find(id);
            if (timecard == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeId", timecard.EmployeeId);
            ViewBag.SkillId = new SelectList(db.JobClassifications, "SkillId", "SkillCode", timecard.SkillId);
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "ProjectNumber", timecard.ProjectId);
            return View(timecard);
        }

        // POST: /TimeCard/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="TimeCardId,EmployeeId,SkillId,ProjectId,Date,TimeIn,TimeOut")] TimeCard timecard)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timecard).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeId", timecard.EmployeeId);
            ViewBag.SkillId = new SelectList(db.JobClassifications, "SkillId", "SkillCode", timecard.SkillId);
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "ProjectNumber", timecard.ProjectId);
            return View(timecard);
        }

        // GET: /TimeCard/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeCard timecard = db.TimeCards.Find(id);
            if (timecard == null)
            {
                return HttpNotFound();
            }
            return View(timecard);
        }

        // POST: /TimeCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TimeCard timecard = db.TimeCards.Find(id);
            db.TimeCards.Remove(timecard);
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
