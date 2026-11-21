using ElderlyCareHospital.Models;
using Microsoft.AspNet.Identity;
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
    public class DoctorNotesController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // GET: DoctorNotes

        public ActionResult Index()
        {
            // Get doctors for displaying FullName
            ViewBag.Doctors = db.Doctors.ToList();

            // Get nurse notes
            var nurseNotes = db.NurseNotes
                .Include(n => n.Patient)
                .OrderByDescending(n => n.DateCreated)
                .ToList();

            // Get doctor notes
            var doctorNotes = db.DoctorNotes
                .Include(d => d.Patient)
                .OrderByDescending(d => d.DateCreated)
                .ToList();

            ViewBag.NurseNotes = nurseNotes;

            return View(doctorNotes);
        }
        // GET: DoctorNotes/GetLiveNotes
        public ActionResult GetLiveNotes()
        {
            ViewBag.Doctors = db.Doctors.ToList();

            var notes = db.DoctorNotes
                          .Include(d => d.Patient)
                          .OrderByDescending(d => d.DateCreated)
                          .ToList();

            return PartialView("_DoctorNotesRows", notes);
        }

        public ActionResult Create()
        {
            // Patient dropdown
            ViewBag.PatientID = new SelectList(
                db.Patients,
                "PatientID",
                "FirstName"
            );

            // Get logged-in user's ID
            string userId = User.Identity.GetUserId();

            // Find the doctor linked to that user
            var currentDoctor = db.Doctors
                                   .FirstOrDefault(d => d.UserID == userId);

            if (currentDoctor == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.Forbidden,
                    "The currently logged-in user is not registered as a doctor."
                );
            }

            // Send FULL NAME to the view
            ViewBag.CurrentDoctorName = currentDoctor.FullName;

            // Automatically assign DoctorID
            var doctorNote = new DoctorNotes
            {
                DoctorID = currentDoctor.DoctorID,
                DateCreated = DateTime.Now
            };

            return View(doctorNote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    [Bind(Include = "PatientID,NoteText")] DoctorNotes note)
        {
            // Get logged-in user's ID
            string userId = User.Identity.GetUserId();

            // Find the doctor
            var currentDoctor = db.Doctors
                                   .FirstOrDefault(d => d.UserID == userId);

            if (currentDoctor == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.Forbidden,
                    "The currently logged-in user is not registered as a doctor."
                );
            }

            // Automatically assign the correct doctor
            note.DoctorID = currentDoctor.DoctorID;

            // Automatically set date
            note.DateCreated = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.DoctorNotes.Add(note);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Reload patient dropdown if validation fails
            ViewBag.PatientID = new SelectList(
                db.Patients,
                "PatientID",
                "FirstName",
                note.PatientID
            );

            // Show doctor's FULL NAME again
            ViewBag.CurrentDoctorName = currentDoctor.FullName;

            return View(note);
        }

        // GET: DoctorNotes/Edit/3
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DoctorNotes note = db.DoctorNotes.Find(id);

            if (note == null)
            {
                return HttpNotFound();
            }

            // Patient dropdown
            ViewBag.PatientID = new SelectList(
                db.Patients,
                "PatientID",
                "FirstName",
                note.PatientID
            );

            // Get doctor name for display
            var doctor = db.Doctors.FirstOrDefault(d => d.DoctorID == note.DoctorID);

            if (doctor != null)
            {
                ViewBag.CurrentDoctorName = doctor.FullName;
            }

            return View(note);
        }

        // POST: DoctorNotes/Edit/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "NoteID,PatientID,NoteText,DoctorID,DateCreated")] DoctorNotes note)
        {
            if (ModelState.IsValid)
            {
                db.Entry(note).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Reload patient dropdown if validation fails
            ViewBag.PatientID = new SelectList(
                db.Patients,
                "PatientID",
                "FirstName",
                note.PatientID
            );

            // Reload doctor name
            var doctor = db.Doctors.FirstOrDefault(d => d.DoctorID == note.DoctorID);

            if (doctor != null)
            {
                ViewBag.CurrentDoctorName = doctor.FullName;
            }

            return View(note);
        }

        // GET: DoctorNotes/Details/3
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DoctorNotes note = db.DoctorNotes
                .Include(d => d.Patient)
                .FirstOrDefault(d => d.NoteID == id);

            if (note == null)
            {
                return HttpNotFound();
            }

            var doctor = db.Doctors
                .FirstOrDefault(d => d.DoctorID == note.DoctorID);

            if (doctor != null)
            {
                ViewBag.CurrentDoctorName = doctor.FullName;
            }

            return View(note);
        }

        // GET: DoctorNotes/Delete/3
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DoctorNotes note = db.DoctorNotes
                .Include(d => d.Patient)
                .FirstOrDefault(d => d.NoteID == id);

            if (note == null)
            {
                return HttpNotFound();
            }

            var doctor = db.Doctors
                .FirstOrDefault(d => d.DoctorID == note.DoctorID);

            if (doctor != null)
            {
                ViewBag.CurrentDoctorName = doctor.FullName;
            }

            return View(note);
        }

        // POST: DoctorNotes/Delete/3
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DoctorNotes note = db.DoctorNotes.Find(id);

            if (note == null)
            {
                return HttpNotFound();
            }

            db.DoctorNotes.Remove(note);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}
