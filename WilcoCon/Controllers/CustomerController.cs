using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Net;

namespace WilcoCon.Models
{
    public class CustomerController : Controller
    {
        private WilcoEntities db = new WilcoEntities();
        //
        // GET: /Customer/
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        //
        // GET: /Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Customer customer = db.Customers.Find(id);
            CustomerDetailViewModel viewmodel = new CustomerDetailViewModel();
            viewmodel.Customer = db.Customers.Find(id);
            if (viewmodel.Customer == null)
            {
                return HttpNotFound();
            }
            viewmodel.Projects = from p in db.Projects
                                 where p.CustomerId == viewmodel.Customer.CustomerId
                                 select p;
            return View(viewmodel);
            //var CustomerProject = from p in db.Projects
            //                      where p.CustomerId == customer.CustomerId
            //                      select p.ProjectNumber;
            //List<string> CustomerProjectList = new List<string>();
            //foreach (string p in CustomerProject)
            //{
            //    CustomerProjectList.Add(p);
            //}
            //ViewBag.CustomerProjectList = CustomerProjectList;
            //return View(customer);
        }

        //
        // GET: /Customer/Create
        public ActionResult Create(FormCollection collection)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompanyName, CustomerId, FirstName, LastName, Email, Phone, PaymentTerms, BillingAddress, BillingCity, BillingState, BillingZip,Notes")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        //
        // POST: /Customer/Create
        

        //
        // GET: /Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        //
        // POST: /Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="CustomerId, FirstName, LastName, Email, Phone, PaymentTerms, BillingAddress,Notes, CompanyName, BillingCity, BillingState, BillingZip")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //
        // GET: /Customer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        //
        // POST: /Customer/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
