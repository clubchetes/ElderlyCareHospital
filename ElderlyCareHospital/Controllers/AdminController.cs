using ElderlyCareHospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace ElderlyCareHospital.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext identityDb = new ApplicationDbContext();
        private MyDatabaseModels db = new MyDatabaseModels();
      

        public ActionResult Users()
        {
            var user = identityDb.Users.ToList();
            return View(user);  
        }

        public ActionResult Inventory()
        {
            var InventoryList = db.Equipments.ToList();
            return View(InventoryList);
        }


        public ActionResult Dashboard()
        {
            using (var db = new ElderlyCareHospital.Models.MyDatabaseModels())
            {
                ViewBag.TotalPatients =
                db.Patients.Count();

                ViewBag.LowStock =
                    db.Medicines
                    .Count(x => x.Quantity <= x.MinimumStock);

                ViewBag.ActiveAlerts =
              db.EmergencyAlerts
                .Count(x => x.Status == "Active");

                ViewBag.TotalRequests =
                db.InventoryRequests
                .Count();

                ViewBag.VisitCount = db.Visits
    .Count(v => v.Status == "Pending");


                // Get registered Identity users from AspNetUsers
                var usersList = identityDb.Users.ToList();

                ViewBag.TotalUsers = usersList.Count;

                return View(usersList);

               
            }



            
        }
        [Authorize(Roles = "Admin")]
        public JsonResult GetDashboardStats()
        {
            using (var db = new ElderlyCareHospital.Models.MyDatabaseModels())
            {
                var totalPatients = db.Patients.Count();

                var lowStock = db.Medicines
                    .Count(x => x.Quantity <= x.MinimumStock);

                var activeAlerts =
            db.EmergencyAlerts
              .Count(x => x.Status == "Active");

                var totalRequests = db.InventoryRequests.Count();

                var visitCount = db.Visits.Count(v => v.Status == "Pending");

                var totalUsers = identityDb.Users.Count();

                return Json(new
                {
                    totalUsers = totalUsers,
                    totalPatients = totalPatients,
                    visitCount = visitCount,
                    lowStock = lowStock,
                    activeAlerts = activeAlerts,
                    totalRequests = totalRequests
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUser(string id)
        {
            var user = identityDb.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                // Delete Family Member record
                var familyMember = db.FamilyMembers
                    .FirstOrDefault(f => f.UserID == id);

                if (familyMember != null)
                {
                    db.FamilyMembers.Remove(familyMember);
                }


                // Delete Doctor record
                var doctor = db.Doctors
                    .FirstOrDefault(d => d.UserID == id);

                if (doctor != null)
                {
                    db.Doctors.Remove(doctor);
                }


                // Delete Nurse record
                var nurse = db.Nurses
                    .FirstOrDefault(n => n.UserID == id);

                if (nurse != null)
                {
                    db.Nurses.Remove(nurse);
                }


                // Delete Pharmacist record
                var pharmacist = db.Pharmacists
                    .FirstOrDefault(p => p.UserID == id);

                if (pharmacist != null)
                {
                    db.Pharmacists.Remove(pharmacist);
                }


                // Delete Cleaner record
                var cleaner = db.Cleaners
                    .FirstOrDefault(c => c.UserID == id);

                if (cleaner != null)
                {
                    db.Cleaners.Remove(cleaner);
                }


                // Save related records first
                db.SaveChanges();


                // Now delete the Identity user
                identityDb.Users.Remove(user);
                identityDb.SaveChanges();
            }

            return RedirectToAction("Index", "Users");
        }

    }
}