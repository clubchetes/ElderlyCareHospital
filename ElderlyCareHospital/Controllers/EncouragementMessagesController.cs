using ElderlyCareHospital.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ElderlyCareHospital.Controllers
{
    [Authorize(Roles = "FamilyMember, Nurse, Doctor")]
    public class EncouragementMessagesController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();


        // =========================================================
        // CREATE - GET
        // =========================================================
        [Authorize]
        public ActionResult Index()
        {
            var messagesQuery = db.Set<EncouragementMessage>()
                                  .AsQueryable();

            // =========================================================
            // NURSE
            // Nurse can see ALL encouragement messages
            // =========================================================

            if (User.IsInRole("Nurse"))
            {
                var allMessages = messagesQuery
                    .OrderByDescending(x => x.DateSent)
                    .ToList();

                ViewBag.Patients = db.Patients
                    .ToDictionary(x => x.PatientID);

                ViewBag.FamilyMembers = db.FamilyMembers
                    .ToDictionary(x => x.FamilyMemberID);

                ViewBag.IsNurse = true;

                return View(allMessages);
            }


            // =========================================================
            // FAMILY MEMBER
            // Only show messages belonging to the logged-in
            // family member.
            // =========================================================

            if (User.IsInRole("FamilyMember"))
            {
                string userId =
                    Microsoft.AspNet.Identity.IdentityExtensions
                        .GetUserId(User.Identity);

                var family = db.FamilyMembers
                               .FirstOrDefault(x => x.UserID == userId);

                if (family == null)
                {
                    return HttpNotFound();
                }

                var familyMessages = messagesQuery
                    .Where(x => x.FamilyID == family.FamilyMemberID)
                    .OrderByDescending(x => x.DateSent)
                    .ToList();

                ViewBag.Patients = db.Patients
                    .ToDictionary(x => x.PatientID);

                ViewBag.FamilyMembers = db.FamilyMembers
                    .ToDictionary(x => x.FamilyMemberID);

                ViewBag.IsNurse = false;

                ViewBag.FamilyMemberName = family.FullName;

                return View(familyMessages);
            }


            // =========================================================
            // OTHER USERS
            // =========================================================

            return new HttpUnauthorizedResult();
        }

        [Authorize(Roles = "FamilyMember")]
        public ActionResult Create()
        {
            string userId =
                Microsoft.AspNet.Identity.IdentityExtensions
                    .GetUserId(User.Identity);

            var family = db.FamilyMembers
                           .FirstOrDefault(x => x.UserID == userId);

            if (family == null)
            {
                return HttpNotFound();
            }

            var patient = db.Patients
                            .Find(family.PatientID);

            if (patient == null)
            {
                return HttpNotFound();
            }

            var message = new EncouragementMessage
            {
                PatientID = family.PatientID,
                FamilyID = family.FamilyMemberID,
                DateSent = DateTime.Now
            };

            ViewBag.PatientName =
                patient.FirstName + " " + patient.LastName;

            ViewBag.FamilyMemberName =
                family.FullName;

            return View(message);
        }


        // =========================================================
        // CREATE - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "FamilyMember")]
        public ActionResult Create(EncouragementMessage message)
        {
            string userId =
                Microsoft.AspNet.Identity.IdentityExtensions
                    .GetUserId(User.Identity);

            var family = db.FamilyMembers
                           .FirstOrDefault(x => x.UserID == userId);

            if (family == null)
            {
                return HttpNotFound();
            }

            var patient = db.Patients
                            .Find(family.PatientID);

            if (patient == null)
            {
                return HttpNotFound();
            }

            // Never trust IDs submitted by the browser.
            message.PatientID = family.PatientID;
            message.FamilyID = family.FamilyMemberID;
            message.DateSent = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Set<EncouragementMessage>().Add(message);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.PatientName =
                patient.FirstName + " " + patient.LastName;

            ViewBag.FamilyMemberName =
                family.FullName;

            return View(message);
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

        // =========================================================
        // DETAILS
        // =========================================================

        [Authorize(Roles = "FamilyMember,Nurse,Doctor")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            var message = db.Set<EncouragementMessage>()
                            .FirstOrDefault(x => x.MessageID == id);

            if (message == null)
            {
                return HttpNotFound();
            }

            // Get patient
            var patient = db.Patients
                            .FirstOrDefault(x => x.PatientID == message.PatientID);

            // Get family member
            var familyMember = db.FamilyMembers
                                 .FirstOrDefault(x =>
                                     x.FamilyMemberID == message.FamilyID);

            // Send data to Details view
            ViewBag.Patient = patient;
            ViewBag.FamilyMember = familyMember;

            return View(message);
        }

        // =========================================================
        // DELETE - GET
        // =========================================================

        [Authorize(Roles = "FamilyMember,Nurse,Doctor")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            var message = db.Set<EncouragementMessage>()
                            .FirstOrDefault(x => x.MessageID == id);

            if (message == null)
            {
                return HttpNotFound();
            }

            // Get patient
            var patient = db.Patients
                            .FirstOrDefault(x =>
                                x.PatientID == message.PatientID);

            // Get family member
            var familyMember = db.FamilyMembers
                                 .FirstOrDefault(x =>
                                     x.FamilyMemberID == message.FamilyID);

            // Send information to Delete view
            ViewBag.Patient = patient;
            ViewBag.FamilyMember = familyMember;

            return View(message);
        }


        // =========================================================
        // DELETE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "FamilyMember,Nurse,Doctor")]
        public ActionResult DeleteConfirmed(int id)
        {
            var message = db.Set<EncouragementMessage>()
                            .FirstOrDefault(x => x.MessageID == id);

            if (message == null)
            {
                return HttpNotFound();
            }

            db.Set<EncouragementMessage>().Remove(message);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}