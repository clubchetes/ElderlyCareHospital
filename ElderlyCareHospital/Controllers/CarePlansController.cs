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
    [Authorize(Roles = "Doctor, Nurse")]
    
    public class CarePlansController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // GET: CarePlans
        public ActionResult Index()
        {
            var carePlans = db.CarePlans.Include(c => c.Patient);
            return View(carePlans.ToList());
        }
        // GET: CarePlans/GetLiveTable
        [Authorize(Roles = "Doctor, Nurse")]
        public ActionResult GetLiveTable()
        {
            var carePlans = db.CarePlans.Include(c => c.Patient).ToList();
            return PartialView("_CarePlanTableRows", carePlans);
        }

        // GET: CarePlans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarePlan carePlan = db.CarePlans.Find(id);
            if (carePlan == null)
            {
                return HttpNotFound();
            }
            return View(carePlan);
        }

        // GET: CarePlans/Create
        public ActionResult Create()
        {
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName");
            return View();
        }

        // POST: CarePlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CarePlanID,PatientID,MedicationRoutine,DietInstructions,MobilitySupport,TherapySchedule,MemoryCareNotes")] CarePlan carePlan)
        {
            if (ModelState.IsValid)
            {
                db.CarePlans.Add(carePlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", carePlan.PatientID);
            return View(carePlan);
        }

        // GET: CarePlans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarePlan carePlan = db.CarePlans.Find(id);
            if (carePlan == null)
            {
                return HttpNotFound();
            }
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", carePlan.PatientID);
            return View(carePlan);
        }

        // POST: CarePlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CarePlanID,PatientID,MedicationRoutine,DietInstructions,MobilitySupport,TherapySchedule,MemoryCareNotes")] CarePlan carePlan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(carePlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", carePlan.PatientID);
            return View(carePlan);
        }

        // GET: CarePlans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarePlan carePlan = db.CarePlans.Find(id);
            if (carePlan == null)
            {
                return HttpNotFound();
            }
            return View(carePlan);
        }

        // POST: CarePlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CarePlan carePlan = db.CarePlans.Find(id);
            db.CarePlans.Remove(carePlan);
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
