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
    [Authorize]
    public class EmergencyAlertsController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // GET: EmergencyAlerts
        public ActionResult Index()
        {
            var emergencyAlerts = db.EmergencyAlerts.Include(e => e.Patient);
            return View(emergencyAlerts.ToList());
        }

        // GET: EmergencyAlerts/GetLiveAlerts
        public ActionResult GetLiveAlerts()
        {
            var emergencyAlerts = db.EmergencyAlerts.Include(e => e.Patient);
            return PartialView("_EmergencyAlertRows", emergencyAlerts.ToList());
        }

        public ActionResult ActiveAlerts()
        {
            var alerts =
                db.EmergencyAlerts
                .Include(x => x.Patient)
                  .Where(x =>
                    x.Status == "Active")
                  .OrderByDescending(x =>
                    x.AlertDate)
                  .ToList();

            return View(alerts);
        }

        [Authorize(Roles =
"Doctor,Admin")]
        public ActionResult Resolve(int id)
        {
            var alert =
                db.EmergencyAlerts
                  .Find(id);

            alert.Status =
                "Resolved";

            alert.ResolvedBy =
                User.Identity.Name;

            alert.ResolvedDate =
                DateTime.Now;

            db.SaveChanges();

            return RedirectToAction(
                "ActiveAlerts");
        }

        public ActionResult History()
        {
            var alerts =
                db.EmergencyAlerts
                  .OrderByDescending(
                     x => x.AlertDate)
                  .ToList();

            return View(alerts);
        }



        // GET: EmergencyAlerts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmergencyAlert emergencyAlert = db.EmergencyAlerts.Find(id);
            if (emergencyAlert == null)
            {
                return HttpNotFound();
            }
            return View(emergencyAlert);
        }

        // GET: EmergencyAlerts/Create
        public ActionResult Create()
        {
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    EmergencyAlert alert)
        {
            if (ModelState.IsValid)
            {
                alert.AlertDate =
                    DateTime.Now;

                alert.Status =
                    "Active";

                db.EmergencyAlerts
                  .Add(alert);

                db.SaveChanges();

                return RedirectToAction(
                    "Index");
            }

            return View(alert);
        }

       

        // GET: EmergencyAlerts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmergencyAlert emergencyAlert = db.EmergencyAlerts.Find(id);
            if (emergencyAlert == null)
            {
                return HttpNotFound();
            }
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", emergencyAlert.PatientID);
            return View(emergencyAlert);
        }

        // POST: EmergencyAlerts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AlertID,PatientID,AlertType,Description,Severity,AlertDate,Status,ResolvedBy,ResolvedDate")] EmergencyAlert emergencyAlert)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emergencyAlert).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PatientID = new SelectList(db.Patients, "PatientID", "FirstName", emergencyAlert.PatientID);
            return View(emergencyAlert);
        }

        // GET: EmergencyAlerts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmergencyAlert emergencyAlert = db.EmergencyAlerts.Find(id);
            if (emergencyAlert == null)
            {
                return HttpNotFound();
            }
            return View(emergencyAlert);
        }

        // POST: EmergencyAlerts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmergencyAlert emergencyAlert = db.EmergencyAlerts.Find(id);
            db.EmergencyAlerts.Remove(emergencyAlert);
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
