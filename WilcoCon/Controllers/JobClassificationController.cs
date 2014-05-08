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
    public class JobClassificationController : Controller
    {
        private WilcoEntities db = new WilcoEntities();

        // GET: /JobClassification/
        [Authorize]
        public ActionResult Index()
        {
            return View(db.JobClassifications.ToList());
        }

        // GET: /JobClassification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobClassification jobclassification = db.JobClassifications.Find(id);
            if (jobclassification == null)
            {
                return HttpNotFound();
            }
            return View(jobclassification);
        }

        // GET: /JobClassification/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /JobClassification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="SkillId,SkillCode,JobType")] JobClassification jobclassification)
        {
            int skillCodeCount = (from j in db.JobClassifications
                                  where jobclassification.SkillCode.ToLower() == j.SkillCode.ToLower()
                                  select j.SkillCode).Count();
            if (skillCodeCount > 0)
            {
                ModelState.AddModelError("", "Skill Code is a duplicate!  Please enter a unique SkillCode");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.JobClassifications.Add(jobclassification);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(jobclassification);
        }

        // GET: /JobClassification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobClassification jobclassification = db.JobClassifications.Find(id);
            if (jobclassification == null)
            {
                return HttpNotFound();
            }
            return View(jobclassification);
        }

        // POST: /JobClassification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="SkillId,SkillCode,JobType")] JobClassification jobclassification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jobclassification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jobclassification);
        }

        // GET: /JobClassification/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobClassification jobclassification = db.JobClassifications.Find(id);
            if (jobclassification == null)
            {
                return HttpNotFound();
            }
            return View(jobclassification);
        }

        // POST: /JobClassification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JobClassification jobclassification = db.JobClassifications.Find(id);
            db.JobClassifications.Remove(jobclassification);
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
