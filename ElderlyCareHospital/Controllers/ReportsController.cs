using ElderlyCareHospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElderlyCareHospital.Controllers
{
    public class ReportsController : Controller
    {

        private MyDatabaseModels db = new MyDatabaseModels();

        public ActionResult PatientStatistics()
        {
            ViewBag.TotalPatients =
                db.Patients.Count();

            ViewBag.HighRiskPatients =
                db.Patients.Count(x => x.FallRisk == true);

            return View();
        }

        // GET: Reports/GetLivePatientStatistics
        public JsonResult GetLivePatientStatistics()
        {
            int totalPatients = db.Patients.Count();
            int highRiskPatients = db.Patients.Count(x => x.FallRisk == true);

            return Json(new { totalPatients = totalPatients, highRiskPatients = highRiskPatients }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InventoryReport()
        {
            var stock =
                db.Medicines.ToList();

            return View(stock);
        }

        public ActionResult VisitReport()
        {
            return View(
                db.Visits.ToList());
        }

        public ActionResult IncidentReport()
        {
            return View(
                db.EmergencyAlerts.ToList());
        }
    }
}