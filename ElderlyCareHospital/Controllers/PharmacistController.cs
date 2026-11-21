using ElderlyCareHospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElderlyCareHospital.Controllers
{
    [Authorize(Roles = "Pharmacist")]
    public class PharmacistController : Controller
    {
     

        private MyDatabaseModels db = new MyDatabaseModels();

   

        public ActionResult Dashboard()
        {
            ViewBag.TotalMedicines =
                db.Medicines.Count();

            ViewBag.LowStock =
                db.Medicines.Count(x =>
                    x.Quantity <= x.MinimumStock);
            DateTime checkDate = DateTime.Now.AddMonths(3);
            ViewBag.ExpiringSoon =
                db.Medicines.Count(x =>
                    x.ExpiryDate <=
                    checkDate);


            ViewBag.PendingRequests =
    db.InventoryRequests
      .Count(x =>
         x.Status == "Pending");

            ViewBag.TotalEquipment =
   db.Equipments.Count();

            ViewBag.LowEquipment =
            db.Equipments.Count(x =>
            x.Quantity <=
            x.MinimumQuantity);

            DateTime maintenanceThreshold = DateTime.Now.AddMonths(-6);
            ViewBag.MaintenanceDue =
            db.Equipments.Count(x =>
            x.LastMaintenanceDate <=
            maintenanceThreshold);


            return View();
        }

        [Authorize(Roles = "Pharmacist")]
        public ActionResult GetPharmacistStatsJson()
        {
            ViewBag.TotalMedicines = db.Medicines.Count();

            ViewBag.LowStock = db.Medicines.Count(x => x.Quantity <= x.MinimumStock);

            DateTime checkDate = DateTime.Now.AddMonths(3);
            ViewBag.ExpiringSoon = db.Medicines.Count(x => x.ExpiryDate <= checkDate);

            ViewBag.PendingRequests = db.InventoryRequests.Count(x => x.Status == "Pending");

            ViewBag.TotalEquipment = db.Equipments.Count();

            ViewBag.LowEquipment = db.Equipments.Count(x => x.Quantity <= x.MinimumQuantity);

            DateTime maintenanceThreshold = DateTime.Now.AddMonths(-6);
            ViewBag.MaintenanceDue = db.Equipments.Count(x => x.LastMaintenanceDate <= maintenanceThreshold);

            return PartialView("_PharmacistStats");
        }

        public ActionResult Reject(int id)
        {
            var request =
                db.InventoryRequests
                  .Find(id);

            request.Status =
                "Rejected";

            db.SaveChanges();

            return RedirectToAction(
                "Index");
        }

       
        public ActionResult Approve(int id)
        {
            var request =
                db.InventoryRequests
                  .Find(id);

            request.Status =
                "Approved";

            db.SaveChanges();

            return RedirectToAction(
                "Index");
        }
        public ActionResult LowStock()
        {
            var lowStock =
                db.Medicines
                .Where(x => x.Quantity <= x.MinimumStock)
                .ToList();

            return View(lowStock);
        }

        public ActionResult ExpiringSoon()
        {
            DateTime future =
                DateTime.Now.AddMonths(3);

            var meds =
                db.Medicines
                .Where(x => x.ExpiryDate <= future)
                .ToList();

            return View(meds);
        }
    }
}