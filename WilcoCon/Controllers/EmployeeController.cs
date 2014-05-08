using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity.Validation;

namespace WilcoCon.Models
{
    public class EmployeeController : Controller
    {
        private WilcoEntities db = new WilcoEntities();

        // GET: /Employee/
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        // GET: /Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: /Employee/Create
        public ActionResult Create()
        {
            List<SelectListItem> Gender = new List<SelectListItem>();
            Gender.Add(new SelectListItem{Text ="Male", Value = "Male"});
            Gender.Add(new SelectListItem { Text = "Female", Value = "Female" });
            ViewBag.Gender = Gender;
            List<SelectListItem> MaritalStatusList = new List<SelectListItem>();
            MaritalStatusList.Add(new SelectListItem { Text = "Single", Value = "Single" });
            MaritalStatusList.Add(new SelectListItem { Text = "Married", Value = "Married" });
            ViewBag.MaritalStatus = MaritalStatusList;
            List<SelectListItem> EEOCode = new List<SelectListItem>();
            EEOCode.Add(new SelectListItem { Text = "0- Non-minority", Value = "0" });
            EEOCode.Add(new SelectListItem { Text = "1- Minority", Value = "1" });
            ViewBag.EEOCode = EEOCode;
            return View();
        }

        // POST: /Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="EmployeeId,SSN,LastName,FirstName,Street,City,State,ZipCode,TelephoneNumber,DOB,Gender,MaritalStatus,EEOCOde,NumberOfDeductions")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
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
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        // GET: /Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> Gender = new List<SelectListItem>();
            Gender.Add(new SelectListItem { Text = "Male", Value = "Male" });
            Gender.Add(new SelectListItem { Text = "Female", Value = "Female" });
            ViewBag.Gender = Gender;
            List<SelectListItem> MaritalStatusList = new List<SelectListItem>();
            MaritalStatusList.Add(new SelectListItem { Text = "Single", Value = "Single" });
            MaritalStatusList.Add(new SelectListItem { Text = "Married", Value = "Married" });
            ViewBag.MaritalStatus = MaritalStatusList;
            return View(employee);
        }
        public ActionResult EmployeePay(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if(employee == null)
            {
                return HttpNotFound();
            }
            Repository repository = new Repository();
            ViewBag.WeekEndDates = repository.WeekEndDates();
            EmployeePay empPay = new EmployeePay();
            empPay = empPay.GetEmployeeReport(employee, DateTime.Now);
            return View(empPay);
        }
        [HttpPost]
        public ActionResult EmployeePay(int? id, DateTime WeekEndDate)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            Repository repository = new Repository();
            ViewBag.WeekEndDates = repository.WeekEndDates();
            EmployeePay empPay = new EmployeePay();
            empPay = empPay.GetEmployeeReport(employee, WeekEndDate);
            return View(empPay);
        }
        // POST: /Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="EmployeeId,SSN,LastName,FirstName,Street,City,State,ZipCode,TelephoneNumber,DOB,Gender,MaritalStatus,EEOCOde,NumberOfDeductions")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
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
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: /Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: /Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
