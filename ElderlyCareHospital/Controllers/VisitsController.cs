
using ElderlyCareHospital.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ElderlyCareHospital.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // ======================================================
        // SHOW VISITS
        // ======================================================
        public ActionResult VisitList()
        {
            // Admin sees all visits
            if (User.IsInRole("Admin"))
            {
                return View(db.Visits.OrderByDescending(v => v.VisitDate).ToList());
            }

            string userId = User.Identity.GetUserId();

            var familyMember = db.FamilyMembers
                                 .FirstOrDefault(f => f.UserID == userId);

            if (familyMember == null)
            {
                return View(new List<Visit>());
            }

            // Only show this family member's visits
            var myVisits = db.Visits
                             .Where(v => v.FamilyID == familyMember.FamilyMemberID)
                             .ToList();

            return View(myVisits);
        }

        // GET: Visits/GetLiveVisits
        public ActionResult GetLiveVisits()
        {
            // Admin sees all visits
            if (User.IsInRole("Admin"))
            {
                return PartialView("_VisitRows", db.Visits.OrderByDescending(v => v.VisitDate).ToList());
            }

            string userId = User.Identity.GetUserId();

            var familyMember = db.FamilyMembers
                               .FirstOrDefault(f => f.UserID == userId);

            if (familyMember == null)
            {
                return PartialView("_VisitRows", new List<Visit>());
            }

            // Only show this family member's visits
            var myVisits = db.Visits
                           .Where(v => v.FamilyID == familyMember.FamilyMemberID)
                           .OrderByDescending(v => v.VisitDate)
                           .ToList();

            return PartialView("_VisitRows", myVisits);
        }
        // ======================================================
        // CREATE
        // ======================================================

        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();

            var familyMember = db.FamilyMembers
                                 .Include("Patient")
                                 .FirstOrDefault(f => f.UserID == userId);

            if (familyMember != null)
            {
                ViewBag.FamilyName = familyMember.FullName;

                // Automatically show patient full name
                ViewBag.PatientName =
                    familyMember.Patient.FirstName + " " +
                    familyMember.Patient.LastName;
            }
            else
            {
                ViewBag.FamilyName = User.Identity.Name;
                ViewBag.PatientName = "No patient assigned";
            }

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Visit visit)
        {
            // Make sure only Yes or No is stored
            if (visit.ParkingSlot != "Yes" &&
                visit.ParkingSlot != "No")
            {
                visit.ParkingSlot = null;
            }

            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();

                var familyMember = db.FamilyMembers
                                     .FirstOrDefault(f => f.UserID == userId);

                if (familyMember == null)
                {
                    ModelState.AddModelError("", "Family member record not found.");
                    ViewBag.FamilyName = User.Identity.Name;
                    return View(visit);
                }

                // Automatically assign the FamilyMemberID
                visit.FamilyID = familyMember.FamilyMemberID;

                // Automatically set status
                visit.Status = "Pending";

                db.Visits.Add(visit);
                db.SaveChanges();

                return RedirectToAction("VisitList");
            }

            return View(visit);
        }

        [Authorize(Roles = "Admin")] 
        public ActionResult Approve(int id)
        { 
            var visit = db.Visits.Find(id); 
            
            if (visit == null)
            { 
                return HttpNotFound(); 
            } 
            
            visit.Status = "Approved"; 
            db.SaveChanges(); 
            
            return RedirectToAction("VisitList"); 
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Reject(int id) 
        { 
            var visit = db.Visits.Find(id); 
            
            if (visit == null) 
            {
                return HttpNotFound(); 
            } 
            
            visit.Status = "Rejected"; 
            db.SaveChanges();
            
            return RedirectToAction("VisitList"); 
        }



        // ======================================================
        // DETAILS
        // ======================================================
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Visit visit = db.Visits.Find(id);

            if (visit == null)
            {
                return HttpNotFound();
            }

            // Security: family can only view their own visits
            if (!User.IsInRole("Admin"))
            {
                string currentUser = User.Identity.Name;

                var family = db.FamilyMembers
                               .FirstOrDefault(f => f.Email == currentUser);

                if (family == null || visit.FamilyID != family.FamilyMemberID)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            return View(visit);
        }

        // ======================================================
        // DELETE
        // ======================================================
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Visit visit = db.Visits.Find(id);

            if (visit == null)
            {
                return HttpNotFound();
            }

            return View(visit);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Visit visit = db.Visits.Find(id);

            if (visit != null)
            {
                db.Visits.Remove(visit);
                db.SaveChanges();
            }

            return RedirectToAction("VisitList");
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

