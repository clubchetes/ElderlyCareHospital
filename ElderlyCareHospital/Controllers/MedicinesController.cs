using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElderlyCareHospital.Models;

namespace ElderlyCareHospital.Controllers
{
    [AllowAnonymous]
    public class MedicinesController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // GET: Medicines
        public ActionResult Index()
        {
            return View(db.Medicines.ToList());
        }

        // GET: Medicines/GetLiveMedicines
        public ActionResult GetLiveMedicines()
        {
            var medicines = db.Medicines.ToList();
            return PartialView("_MedicineInventoryRows", medicines);
        }

        // GET: Medicines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medicine medicine = db.Medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return View(medicine);
        }

        public ActionResult LowStock()
        {
            var medicines =
                db.Medicines
                .Where(x =>
                    x.Quantity <= x.MinimumStock)
                .ToList();

            return View(medicines);
        }

        public ActionResult ExpiringSoon()
        {
            DateTime date =
                DateTime.Now.AddMonths(3);

            var medicines =
                db.Medicines
                .Where(x =>
                    x.ExpiryDate <= date)
                .ToList();

            return View(medicines);
        }

        // GET: Medicines/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Medicines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MedicineID,MedicineName,Description,Quantity,ExpiryDate,MinimumStock,Supplier")] Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                db.Medicines.Add(medicine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(medicine);
        }

        // GET: Medicines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medicine medicine = db.Medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return View(medicine);
        }

        // POST: Medicines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MedicineID,MedicineName,Description,Quantity,ExpiryDate,MinimumStock,Supplier")] Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(medicine);
        }

        // GET: Medicines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medicine medicine = db.Medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return View(medicine);
        }

        // POST: Medicines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Medicine medicine = db.Medicines.Find(id);
            db.Medicines.Remove(medicine);
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
