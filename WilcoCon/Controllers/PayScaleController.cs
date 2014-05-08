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
    public class PayScaleController : Controller
    {
        private WilcoEntities db = new WilcoEntities();

        // GET: /PayScale/
        public ActionResult Index()
        {
            var payscales = db.PayScales.Include(p => p.JobClassification).Include(p => p.Project);
            return View(payscales.ToList());
        }

        // GET: /PayScale/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayScale payscale = db.PayScales.Find(id);
            if (payscale == null)
            {
                return HttpNotFound();
            }
            return View(payscale);
        }

        // GET: /PayScale/Create
        public ActionResult Create()
        {
            ViewBag.SkillId = new SelectList(db.JobClassifications, "SkillId", "SkillCode");
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "ProjectNumber");
            return View();
        }

        // POST: /PayScale/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="PayScaleId,BasicHourlyRate,FringeBenefitPayments,SkillId,ProjectId")] PayScale payscale)
        {
            if (ModelState.IsValid)
            {
                db.PayScales.Add(payscale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SkillId = new SelectList(db.JobClassifications, "SkillId", "SkillCode", payscale.SkillId);
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "ProjectNumber", payscale.ProjectId);
            return View(payscale);
        }

        // GET: /PayScale/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayScale payscale = db.PayScales.Find(id);
            if (payscale == null)
            {
                return HttpNotFound();
            }
            ViewBag.SkillId = new SelectList(db.JobClassifications, "SkillId", "SkillCode", payscale.SkillId);
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "ProjectNumber", payscale.ProjectId);
            return View(payscale);
        }

        // POST: /PayScale/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="PayScaleId,BasicHourlyRate,FringeBenefitPayments,SkillId,ProjectId")] PayScale payscale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payscale).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SkillId = new SelectList(db.JobClassifications, "SkillId", "SkillCode", payscale.SkillId);
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "ProjectNumber", payscale.ProjectId);
            return View(payscale);
        }

        // GET: /PayScale/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayScale payscale = db.PayScales.Find(id);
            if (payscale == null)
            {
                return HttpNotFound();
            }
            return View(payscale);
        }

        // POST: /PayScale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PayScale payscale = db.PayScales.Find(id);
            db.PayScales.Remove(payscale);
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
