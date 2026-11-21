using ElderlyCareHospital.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ElderlyCareHospital.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // =====================================================
        // INBOX
        // SHOW RECEIVED AND SENT MESSAGES
        // =====================================================
        public ActionResult Inbox()
        {
            string userId = User.Identity.GetUserId();

            List<Message> receivedMessages = new List<Message>();
            List<Message> sentMessages = new List<Message>();

            // =================================================
            // FAMILY MEMBER
            // =================================================
            if (User.IsInRole("FamilyMember"))
            {
                var familyMember = db.FamilyMembers
                    .FirstOrDefault(f => f.UserID == userId);

                if (familyMember != null)
                {
                    // RECEIVED FROM DOCTORS
                    receivedMessages = db.Messages
                        .Where(m => m.ReceiverID == familyMember.FamilyMemberID)
                        .OrderByDescending(m => m.SentDate)
                        .ToList();

                    // SENT TO DOCTORS
                    sentMessages = db.Messages
                        .Where(m => m.SenderID == familyMember.FamilyMemberID)
                        .OrderByDescending(m => m.SentDate)
                        .ToList();

                    // -----------------------------------------
                    // GET DOCTOR NAMES FOR RECEIVED MESSAGES
                    // -----------------------------------------
                    var doctorNames = db.Doctors
                        .ToDictionary(
                            d => d.DoctorID,
                            d => d.FullName
                        );

                    ViewBag.PersonNames = doctorNames;
                }
            }

            // =================================================
            // DOCTOR
            // =================================================
            else if (User.IsInRole("Doctor"))
            {
                var doctor = db.Doctors
                    .FirstOrDefault(d => d.UserID == userId);

                if (doctor != null)
                {
                    // RECEIVED FROM FAMILY MEMBERS
                    receivedMessages = db.Messages
                        .Where(m => m.ReceiverID == doctor.DoctorID)
                        .OrderByDescending(m => m.SentDate)
                        .ToList();

                    // SENT TO FAMILY MEMBERS
                    sentMessages = db.Messages
                        .Where(m => m.SenderID == doctor.DoctorID)
                        .OrderByDescending(m => m.SentDate)
                        .ToList();

                    // -----------------------------------------
                    // GET FAMILY MEMBER NAMES
                    // -----------------------------------------
                    var familyMemberNames = db.FamilyMembers
                        .ToDictionary(
                            f => f.FamilyMemberID,
                            f => f.FullName
                        );

                    ViewBag.PersonNames = familyMemberNames;
                }
            }

            // =================================================
            // SENT MESSAGES
            // =================================================
            ViewBag.SentMessages = sentMessages;

            // RECEIVED MESSAGES ARE THE MAIN MODEL
            return View(receivedMessages);
        }


        // =====================================================
        // GET LIVE RECEIVED INBOX
        // =====================================================
        public ActionResult GetLiveInbox()
        {
            string userId = User.Identity.GetUserId();

            List<Message> receivedMessages = new List<Message>();

            // =================================================
            // FAMILY MEMBER
            // =================================================
            if (User.IsInRole("FamilyMember"))
            {
                var familyMember = db.FamilyMembers
                    .FirstOrDefault(f => f.UserID == userId);

                if (familyMember != null)
                {
                    receivedMessages = db.Messages
                        .Where(m => m.ReceiverID == familyMember.FamilyMemberID)
                        .OrderByDescending(m => m.SentDate)
                        .ToList();

                    ViewBag.PersonNames = db.Doctors
                        .ToDictionary(
                            d => d.DoctorID,
                            d => d.FullName
                        );
                }
            }

            // =================================================
            // DOCTOR
            // =================================================
            else if (User.IsInRole("Doctor"))
            {
                var doctor = db.Doctors
                    .FirstOrDefault(d => d.UserID == userId);

                if (doctor != null)
                {
                    receivedMessages = db.Messages
                        .Where(m => m.ReceiverID == doctor.DoctorID)
                        .OrderByDescending(m => m.SentDate)
                        .ToList();

                    ViewBag.PersonNames = db.FamilyMembers
                        .ToDictionary(
                            f => f.FamilyMemberID,
                            f => f.FullName
                        );
                }
            }

            return PartialView(
                "_InboxMessageRows",
                receivedMessages
            );
        }


        // =====================================================
        // GET LIVE SENT MESSAGES
        // =====================================================
        public ActionResult GetLiveSentMessages()
        {
            string userId = User.Identity.GetUserId();

            List<Message> sentMessages = new List<Message>();

            // =================================================
            // FAMILY MEMBER
            // =================================================
            if (User.IsInRole("FamilyMember"))
            {
                var familyMember = db.FamilyMembers
                    .FirstOrDefault(f => f.UserID == userId);

                if (familyMember != null)
                {
                    sentMessages = db.Messages
                        .Where(m => m.SenderID == familyMember.FamilyMemberID)
                        .OrderByDescending(m => m.SentDate)
                        .ToList();

                    ViewBag.PersonNames = db.Doctors
                        .ToDictionary(
                            d => d.DoctorID,
                            d => d.FullName
                        );
                }
            }

            // =================================================
            // DOCTOR
            // =================================================
            else if (User.IsInRole("Doctor"))
            {
                var doctor = db.Doctors
                    .FirstOrDefault(d => d.UserID == userId);

                if (doctor != null)
                {
                    sentMessages = db.Messages
                        .Where(m => m.SenderID == doctor.DoctorID)
                        .OrderByDescending(m => m.SentDate)
                        .ToList();

                    ViewBag.PersonNames = db.FamilyMembers
                        .ToDictionary(
                            f => f.FamilyMemberID,
                            f => f.FullName
                        );
                }
            }

            return PartialView(
                "_SentMessageRows",
                sentMessages
            );
        }


        // =====================================================
        // CREATE - GET
        // =====================================================
        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();

            // =================================================
            // FAMILY MEMBER SENDING TO DOCTOR
            // =================================================
            if (User.IsInRole("FamilyMember"))
            {
                var familyMember = db.FamilyMembers
                    .FirstOrDefault(f => f.UserID == userId);

                ViewBag.SenderName = familyMember != null
                    ? familyMember.FullName
                    : User.Identity.Name;

                // ---------------------------------------------
                // GET DISTINCT SPECIALITIES
                // ---------------------------------------------
                var specialities = db.Doctors
                    .Where(d => !string.IsNullOrEmpty(d.Speciality))
                    .Select(d => d.Speciality)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                ViewBag.Specialities = new SelectList(
                    specialities
                );

                // ---------------------------------------------
                // DOCTOR LIST STARTS EMPTY
                // ---------------------------------------------
                ViewBag.Receivers = new SelectList(
                    new List<Doctor>(),
                    "DoctorID",
                    "FullName"
                );
            }

            // =================================================
            // DOCTOR SENDING TO FAMILY MEMBER
            // =================================================
            else if (User.IsInRole("Doctor"))
            {
                var doctor = db.Doctors
                    .FirstOrDefault(d => d.UserID == userId);

                ViewBag.SenderName = doctor != null
                    ? doctor.FullName
                    : User.Identity.Name;

                ViewBag.Receivers = new SelectList(
                    db.FamilyMembers
                        .OrderBy(f => f.FullName)
                        .ToList(),
                    "FamilyMemberID",
                    "FullName"
                );
            }

            return View();
        }


        // =====================================================
        // GET DOCTORS BY SPECIALITY
        // =====================================================
        [HttpGet]
        public JsonResult GetDoctorsBySpeciality(string speciality)
        {
            if (string.IsNullOrWhiteSpace(speciality))
            {
                return Json(
                    new List<object>(),
                    JsonRequestBehavior.AllowGet
                );
            }

            var doctors = db.Doctors
                .Where(d =>
                    d.Speciality == speciality &&
                    !string.IsNullOrEmpty(d.FullName)
                )
                .OrderBy(d => d.FullName)
                .Select(d => new
                {
                    DoctorID = d.DoctorID,
                    FullName = d.FullName
                })
                .ToList();

            return Json(
                doctors,
                JsonRequestBehavior.AllowGet
            );
        }


        // =====================================================
        // CREATE - POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Message message, string speciality)
        {
            string userId = User.Identity.GetUserId();

            // =================================================
            // FAMILY MEMBER
            // =================================================
            if (User.IsInRole("FamilyMember"))
            {
                var familyMember = db.FamilyMembers
                    .FirstOrDefault(f => f.UserID == userId);

                if (familyMember == null)
                {
                    ModelState.AddModelError(
                        "",
                        "Family member record not found."
                    );
                }
                else
                {
                    // -----------------------------------------
                    // SPECIALITY REQUIRED
                    // -----------------------------------------
                    if (string.IsNullOrWhiteSpace(speciality))
                    {
                        ModelState.AddModelError(
                            "speciality",
                            "Please select a doctor speciality."
                        );
                    }

                    // -----------------------------------------
                    // DOCTOR REQUIRED
                    // -----------------------------------------
                    if (!message.ReceiverID.HasValue)
                    {
                        ModelState.AddModelError(
                            "ReceiverID",
                            "Please select a doctor."
                        );
                    }

                    // -----------------------------------------
                    // VERIFY DOCTOR
                    // -----------------------------------------
                    if (
                        message.ReceiverID.HasValue &&
                        !string.IsNullOrWhiteSpace(speciality)
                    )
                    {
                        var selectedDoctor = db.Doctors
                            .FirstOrDefault(d =>
                                d.DoctorID ==
                                message.ReceiverID.Value
                            );

                        if (selectedDoctor == null)
                        {
                            ModelState.AddModelError(
                                "ReceiverID",
                                "The selected doctor could not be found."
                            );
                        }
                        else if (
                            !string.Equals(
                                selectedDoctor.Speciality,
                                speciality,
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            ModelState.AddModelError(
                                "ReceiverID",
                                "The selected doctor does not belong to the selected speciality."
                            );
                        }
                    }

                    // -----------------------------------------
                    // ASSIGN SENDER
                    // -----------------------------------------
                    message.SenderID =
                        familyMember.FamilyMemberID;
                }
            }

            // =================================================
            // DOCTOR
            // =================================================
            else if (User.IsInRole("Doctor"))
            {
                var doctor = db.Doctors
                    .FirstOrDefault(d => d.UserID == userId);

                if (doctor == null)
                {
                    ModelState.AddModelError(
                        "",
                        "Doctor record not found."
                    );
                }
                else
                {
                    message.SenderID =
                        doctor.DoctorID;
                }
            }

            // =================================================
            // SAVE
            // =================================================
            if (ModelState.IsValid)
            {
                message.SentDate = DateTime.Now;

                db.Messages.Add(message);

                db.SaveChanges();

                TempData["Success"] =
                    "Message sent successfully!";

                return RedirectToAction("Inbox");
            }

            // =================================================
            // RELOAD FORM AFTER VALIDATION ERROR
            // =================================================
            if (User.IsInRole("FamilyMember"))
            {
                var familyMember = db.FamilyMembers
                    .FirstOrDefault(f => f.UserID == userId);

                ViewBag.SenderName = familyMember != null
                    ? familyMember.FullName
                    : User.Identity.Name;

                // Reload specialities
                var specialities = db.Doctors
                    .Where(d => !string.IsNullOrEmpty(d.Speciality))
                    .Select(d => d.Speciality)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                ViewBag.Specialities = new SelectList(
                    specialities,
                    speciality
                );

                // -----------------------------------------
                // ONLY SHOW DOCTORS FROM SELECTED SPECIALITY
                // -----------------------------------------
                var doctors = db.Doctors
                    .Where(d =>
                        !string.IsNullOrEmpty(d.FullName) &&
                        d.Speciality == speciality
                    )
                    .OrderBy(d => d.FullName)
                    .ToList();

                ViewBag.Receivers = new SelectList(
                    doctors,
                    "DoctorID",
                    "FullName",
                    message.ReceiverID
                );
            }
            else if (User.IsInRole("Doctor"))
            {
                ViewBag.Receivers = new SelectList(
                    db.FamilyMembers
                        .OrderBy(f => f.FullName)
                        .ToList(),
                    "FamilyMemberID",
                    "FullName",
                    message.ReceiverID
                );

                var doctor = db.Doctors
                    .FirstOrDefault(d => d.UserID == userId);

                ViewBag.SenderName = doctor != null
                    ? doctor.FullName
                    : User.Identity.Name;
            }

            return View(message);
        }


        // =====================================================
        // DETAILS
        // =====================================================
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            Message message = db.Messages.Find(id);

            if (message == null)
            {
                return HttpNotFound();
            }

            return View(message);
        }


        // =====================================================
        // DELETE
        // =====================================================
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            Message message = db.Messages.Find(id);

            if (message == null)
            {
                return HttpNotFound();
            }

            return View(message);
        }


        // =====================================================
        // DELETE CONFIRMED
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);

            if (message != null)
            {
                db.Messages.Remove(message);

                db.SaveChanges();
            }

            TempData["Success"] =
                "Message deleted successfully!";

            return RedirectToAction("Inbox");
        }


        // =====================================================
        // DISPOSE
        // =====================================================
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