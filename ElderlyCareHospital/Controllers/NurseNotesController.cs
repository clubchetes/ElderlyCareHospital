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
    public class NurseNotesController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // =========================================================
        // GET: NurseNotes
        // =========================================================
        public ActionResult Index()
        {
            // Nurse Notes
            var nurseNotes = db.NurseNotes
                .Include(n => n.Patient)
                .OrderByDescending(n => n.DateCreated)
                .ToList();

            // Doctor Notes
            var doctorNotes = db.DoctorNotes
                .Include(d => d.Patient)
                .OrderByDescending(d => d.DateCreated)
                .ToList();

            // Send Doctor Notes to the View
            ViewBag.DoctorNotes = doctorNotes;

            return View(nurseNotes);
        }


        // =========================================================
        // GET: NurseNotes/GetLiveNurseNotes
        // =========================================================
        public ActionResult GetLiveNurseNotes()
        {
            var nurseNotes = db.NurseNotes
                .Include(n => n.Patient)
                .OrderByDescending(n => n.DateCreated)
                .ToList();

            return PartialView("_NurseNotesRows", nurseNotes);
        }


        // =========================================================
        // GET: NurseNotes/GetLiveDoctorNotes
        // =========================================================
        public ActionResult GetLiveDoctorNotes()
        {
            var doctorNotes = db.DoctorNotes
                .Include(d => d.Patient)
                .OrderByDescending(d => d.DateCreated)
                .ToList();

            return PartialView("_DoctorNotesRows", doctorNotes);
        }


        // =========================================================
        // GET: NurseNotes/Details/5
        // =========================================================
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NurseNote nurseNote = db.NurseNotes
                .Include(n => n.Patient)
                .FirstOrDefault(n => n.NoteID == id);

            if (nurseNote == null)
            {
                return HttpNotFound();
            }

            return View(nurseNote);
        }


        // =========================================================
        // GET: NurseNotes/Create
        // =========================================================
        public ActionResult Create()
        {
            ViewBag.PatientID =
                new SelectList(db.Patients, "PatientID", "FirstName");

            var nurseNote = new NurseNote
            {
                DateCreated = DateTime.Now
            };

            return View(nurseNote);
        }


        // =========================================================
        // POST: NurseNotes/Create
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "NoteID,NoteText,DateCreated,PatientID")]
            NurseNote nurseNote)
        {
            if (ModelState.IsValid)
            {
                db.NurseNotes.Add(nurseNote);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.PatientID =
                new SelectList(
                    db.Patients,
                    "PatientID",
                    "FirstName",
                    nurseNote.PatientID
                );

            return View(nurseNote);
        }


        // =========================================================
        // GET: NurseNotes/Edit/5
        // =========================================================
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NurseNote nurseNote = db.NurseNotes.Find(id);

            if (nurseNote == null)
            {
                return HttpNotFound();
            }

            ViewBag.PatientID =
                new SelectList(
                    db.Patients,
                    "PatientID",
                    "FirstName",
                    nurseNote.PatientID
                );

            return View(nurseNote);
        }


        // =========================================================
        // POST: NurseNotes/Edit/5
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "NoteID,NoteText,DateCreated,PatientID")]
            NurseNote nurseNote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nurseNote).State =
                    EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.PatientID =
                new SelectList(
                    db.Patients,
                    "PatientID",
                    "FirstName",
                    nurseNote.PatientID
                );

            return View(nurseNote);
        }


        // =========================================================
        // GET: NurseNotes/Delete/5
        // =========================================================
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            NurseNote nurseNote = db.NurseNotes
                .Include(n => n.Patient)
                .FirstOrDefault(n => n.NoteID == id);

            if (nurseNote == null)
            {
                return HttpNotFound();
            }

            return View(nurseNote);
        }


        // =========================================================
        // POST: NurseNotes/Delete/5
        // =========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NurseNote nurseNote =
                db.NurseNotes.Find(id);

            if (nurseNote != null)
            {
                db.NurseNotes.Remove(nurseNote);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        // =========================================================
        // DISPOSE
        // =========================================================
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