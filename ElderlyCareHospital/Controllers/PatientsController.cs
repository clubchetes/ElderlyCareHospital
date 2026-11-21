using ElderlyCareHospital.Models;
using ElderlyCareHospital.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ElderlyCareHospital.Controllers
{
    public class PatientsController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        public ActionResult Profile(int id)
        {
            var vm = new PatientProfileViewModel();

            vm.Patient = db.Patients.Find(id);

            vm.VitalSigns =
                db.VitalSigns
                .Where(x => x.PatientID == id)
                .OrderByDescending(x => x.DateRecorded)
                .ToList();

            vm.DoctorNotes =
                db.DoctorNotes
                .Where(x => x.PatientID == id)
                .ToList();

            vm.NurseNotes =
                db.NurseNotes
                .Where(x => x.PatientID == id)
                .ToList();

            vm.CarePlan =
                db.CarePlans
                .FirstOrDefault(x => x.PatientID == id);

            return View(vm);
        }

        // GET: Patients/GetLivePatients
        public ActionResult GetLivePatients(string searchString, string genderFilter, string roomFilter, string fallRiskFilter)
        {
            var patients = db.Patients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                patients = patients.Where(p =>
                    p.FirstName.Contains(searchString) ||
                    p.LastName.Contains(searchString) ||
                    p.RoomNumber.Contains(searchString) ||
                    p.EmergencyContact.Contains(searchString) ||
                    p.Allergies.Contains(searchString));
            }

            if (!string.IsNullOrWhiteSpace(genderFilter))
            {
                patients = patients.Where(p => p.Gender == genderFilter);
            }

            if (!string.IsNullOrWhiteSpace(roomFilter))
            {
                patients = patients.Where(p => p.RoomNumber.Contains(roomFilter));
            }

            if (!string.IsNullOrWhiteSpace(fallRiskFilter))
            {
                bool isRisk = bool.Parse(fallRiskFilter);
                patients = patients.Where(p => p.FallRisk == isRisk);
            }

            return PartialView("_PatientRows", patients.ToList());
        }
        // GET: Patients
        public ActionResult Index(string searchString,
                           string genderFilter,
                           string roomFilter,
                           string fallRiskFilter)
        {
            var patients = db.Patients.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                patients = patients.Where(p =>
                    p.FirstName.Contains(searchString) ||
                    p.LastName.Contains(searchString) ||
                    p.RoomNumber.Contains(searchString) ||
                    p.EmergencyContact.Contains(searchString) ||
                    p.Allergies.Contains(searchString));
            }

            // Gender filter
            if (!string.IsNullOrWhiteSpace(genderFilter))
            {
                patients = patients.Where(p => p.Gender == genderFilter);
            }

            // Room filter
            if (!string.IsNullOrWhiteSpace(roomFilter))
            {
                patients = patients.Where(p => p.RoomNumber.Contains(roomFilter));
            }

            // Fall Risk filter
            if (!string.IsNullOrWhiteSpace(fallRiskFilter))
            {
                bool isRisk = bool.Parse(fallRiskFilter);
                patients = patients.Where(p => p.FallRisk == isRisk);
            }

            return View(patients.ToList());
        }

        // GET: Patients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // GET: Patients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatientID,FirstName,LastName,DateOfBirth,Gender,RoomNumber,AdmissionDate,EmergencyContact,Allergies,MedicalHistory,FallRisk")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Patients.Add(patient);
                db.SaveChanges();
                AuditLog log = new AuditLog();

                log.UserName = User.Identity.Name;

                log.ActionPerformed =
                    "Created Patient Record";

                log.ActionDate = DateTime.Now;

                db.AuditLogs.Add(log);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(patient);
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Patient patient = db.Patients.Find(id);

            if (patient == null)
            {
                return HttpNotFound();
            }

            // Get all occupied rooms except the current patient's room
            var occupiedRooms = db.Patients
                .Where(p => p.PatientID != patient.PatientID
                         && !string.IsNullOrEmpty(p.RoomNumber))
                .Select(p => p.RoomNumber)
                .ToList();

            ViewBag.OccupiedRooms = occupiedRooms;

            return View(patient);
        }
        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientID,FirstName,LastName,DateOfBirth,Gender,RoomNumber,AdmissionDate,EmergencyContact,Allergies,MedicalHistory,FallRisk")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            db.Patients.Remove(patient);
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
