using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElderlyCareHospital.Models;
using Microsoft.AspNet.Identity;

namespace ElderlyCareHospital.Controllers
{
    public class MedicationAdministrationsController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // GET: MedicationAdministrations
        public ActionResult Index()
        {
            var medicationAdministrations = db.MedicationAdministrations.Include(m => m.Medicine).Include(m => m.Patient);
            return View(medicationAdministrations.ToList());
        }

        // GET: MedicationAdministrations/GetLiveMedicationAdministrations
        public ActionResult GetLiveMedicationAdministrations()
        {
            var medicationAdministrations = db.MedicationAdministrations
                                              .Include(m => m.Medicine)
                                              .Include(m => m.Patient)
                                              .ToList();
            return PartialView("_MedicationAdministrationRows", medicationAdministrations);
        }

        // GET: MedicationAdministrations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicationAdministration medicationAdministration = db.MedicationAdministrations.Find(id);
            if (medicationAdministration == null)
            {
                return HttpNotFound();
            }
            return View(medicationAdministration);
        }

        // GET: MedicationAdministrations/Create
        // GET: MedicationAdministrations/Create
        public ActionResult Create()
        {
            ViewBag.MedicineID = new SelectList(
                db.Medicines,
                "MedicineID",
                "MedicineName");

            ViewBag.PatientID = new SelectList(
                db.Patients,
                "PatientID",
                "FirstName");

            var medication = new MedicationAdministration

            
            {
                // Automatically set current date and time
                DateAdministered = DateTime.Now
            };

            // Get the logged-in ASP.NET Identity user ID
            string currentUserId = User.Identity.GetUserId();

            // Find the nurse linked to that user
            var currentNurse = db.Nurses
                                 .FirstOrDefault(n => n.UserID == currentUserId);

            if (currentNurse != null)
            {
                medication.NurseID = currentNurse.NurseID;

                // Send FullName to the view
                ViewBag.CurrentNurseName = currentNurse.FullName;
            }

            return View(medication);
        }

        // POST: MedicationAdministrations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AdministrationID,PatientID,MedicineID,NurseID,Status,DateAdministered")] MedicationAdministration medicationAdministration)
        {
            medicationAdministration.DateAdministered = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.MedicationAdministrations.Add(medicationAdministration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName", medicationAdministration.MedicineID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", medicationAdministration.PatientID);
            return View(medicationAdministration);
        }

        // GET: MedicationAdministrations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicationAdministration medicationAdministration = db.MedicationAdministrations.Find(id);
            if (medicationAdministration == null)
            {
                return HttpNotFound();
            }
            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName", medicationAdministration.MedicineID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", medicationAdministration.PatientID);
            return View(medicationAdministration);
        }

        // POST: MedicationAdministrations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdministrationID,PatientID,MedicineID,NurseID,Status,DateAdministered")] MedicationAdministration medicationAdministration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicationAdministration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName", medicationAdministration.MedicineID);
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", medicationAdministration.PatientID);
            return View(medicationAdministration);
        }

        // GET: MedicationAdministrations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicationAdministration medicationAdministration = db.MedicationAdministrations.Find(id);
            if (medicationAdministration == null)
            {
                return HttpNotFound();
            }
            return View(medicationAdministration);
        }

        // POST: MedicationAdministrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MedicationAdministration medicationAdministration = db.MedicationAdministrations.Find(id);
            db.MedicationAdministrations.Remove(medicationAdministration);
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
