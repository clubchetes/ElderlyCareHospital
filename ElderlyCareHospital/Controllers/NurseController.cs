using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ElderlyCareHospital.Models;


namespace ElderlyCareHospital.Controllers
{
    [Authorize(Roles = "Nurse")]
    public class NurseController : Controller
    {
      

        public ActionResult Dashboard()
        {

            using (var db = new ElderlyCareHospital.Models.MyDatabaseModels())
            {
                ViewBag.Patients =
           db.Patients.Count();

                ViewBag.Vitals =
                    db.VitalSigns.Count();

                ViewBag.Medications =
                    db.MedicationAdministrations.Count();

                ViewBag.ActiveAlerts =
    db.EmergencyAlerts
      .Count(x =>
         x.Status == "Active");

                ViewBag.PendingRequests =
    db.InventoryRequests
      .Count(x =>
         x.Status == "Pending");

                return View();
            }

        }

        [Authorize(Roles = "Nurse")]
        public ActionResult GetNurseStatsJson()
        {
            using (var db = new ElderlyCareHospital.Models.MyDatabaseModels())
            {
                ViewBag.Patients = db.Patients.Count();
                ViewBag.Vitals = db.VitalSigns.Count();
                ViewBag.Medications = db.MedicationAdministrations.Count();

                ViewBag.ActiveAlerts = db.EmergencyAlerts.Count(x => x.Status == "Active");
                ViewBag.PendingRequests = db.InventoryRequests.Count(x => x.Status == "Pending");

                return PartialView("_NurseStats");
            }
        }
        public ActionResult Create()
        {
            return View();
        }

    }
}