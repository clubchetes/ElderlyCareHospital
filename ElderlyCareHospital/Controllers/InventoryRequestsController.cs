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
    public class InventoryRequestsController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();


        // =========================================================
        // INDEX
        // =========================================================

        public ActionResult Index()
        {
            // Admin and Pharmacist can see all requests
            if (User.IsInRole("Admin") || User.IsInRole("Pharmacist"))
            {
                return View(
                    db.InventoryRequests
                      .OrderByDescending(x => x.RequestDate)
                      .ToList()
                );
            }

            // Current logged-in user
            string currentUser = User.Identity.Name;

            var requests = db.InventoryRequests
                             .Where(x => x.RequestedBy == currentUser)
                             .OrderByDescending(x => x.RequestDate)
                             .ToList();

            return View(requests);
        }


        // =========================================================
        // LIVE INVENTORY REQUESTS
        // =========================================================

        public ActionResult GetLiveInventoryRequests()
        {
            IEnumerable<InventoryRequest> requests;

            if (User.IsInRole("Admin") || User.IsInRole("Pharmacist"))
            {
                requests = db.InventoryRequests
                             .OrderByDescending(x => x.RequestDate)
                             .ToList();
            }
            else
            {
                string currentUser = User.Identity.Name;

                requests = db.InventoryRequests
                             .Where(x => x.RequestedBy == currentUser)
                             .OrderByDescending(x => x.RequestDate)
                             .ToList();
            }

            return PartialView("_InventoryRequestRows", requests);
        }


        // =========================================================
        // DETAILS
        // =========================================================

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            InventoryRequest inventoryRequest =
                db.InventoryRequests.Find(id);

            if (inventoryRequest == null)
            {
                return HttpNotFound();
            }

            return View(inventoryRequest);
        }


        // =========================================================
        // CREATE - GET
        // =========================================================

        public ActionResult Create()
        {
            LoadInventoryItems();

            return View();
        }


        // =========================================================
        // CREATE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InventoryRequest request)
        {
            // -----------------------------------------------------
            // Validate item name
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(request.ItemName))
            {
                ModelState.AddModelError(
                    "ItemName",
                    "Please select a medicine or equipment."
                );
            }


            // -----------------------------------------------------
            // Validate quantity
            // -----------------------------------------------------

            if (request.QuantityRequested <= 0)
            {
                ModelState.AddModelError(
                    "QuantityRequested",
                    "Please enter a quantity greater than zero."
                );
            }


            // -----------------------------------------------------
            // CHECK INVENTORY STOCK
            // -----------------------------------------------------

            if (ModelState.IsValid)
            {
                string itemName = request.ItemName.Trim();

                // Check medicine first
                var medicine = db.Medicines
                    .FirstOrDefault(x =>
                        x.MedicineName == itemName);

                // If medicine exists
                if (medicine != null)
                {
                    if (medicine.Quantity < request.QuantityRequested)
                    {
                        ModelState.AddModelError(
                            "QuantityRequested",
                            "Insufficient medicine stock. " +
                            "Available stock: " +
                            medicine.Quantity
                        );
                    }
                }
                else
                {
                    // Check equipment
                    var equipment = db.Equipments
                        .FirstOrDefault(x =>
                            x.EquipmentName == itemName);

                    if (equipment != null)
                    {
                        if (equipment.Quantity < request.QuantityRequested)
                        {
                            ModelState.AddModelError(
                                "QuantityRequested",
                                "Insufficient equipment quantity. " +
                                "Available quantity: " +
                                equipment.Quantity
                            );
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(
                            "ItemName",
                            "The selected item could not be found in the hospital inventory."
                        );
                    }
                }
            }


            // -----------------------------------------------------
            // SAVE REQUEST
            // -----------------------------------------------------

            if (ModelState.IsValid)
            {
                request.RequestDate = DateTime.Now;

                // Logged-in user
                request.RequestedBy = User.Identity.Name;

                // Every new request starts as Pending
                request.Status = "Pending";

                db.InventoryRequests.Add(request);

                db.SaveChanges();

                return RedirectToAction("Index");
            }


            // -----------------------------------------------------
            // VALIDATION FAILED
            // -----------------------------------------------------

            LoadInventoryItems();

            return View(request);
        }


        // =========================================================
        // LOAD MEDICINES AND EQUIPMENT
        // =========================================================

        private void LoadInventoryItems()
        {
            var medicines = db.Medicines
                              .Where(x => x.Quantity > 0)
                              .OrderBy(x => x.MedicineName)
                              .ToList();

            var equipment = db.Equipments
                              .Where(x => x.Quantity > 0)
                              .OrderBy(x => x.EquipmentName)
                              .ToList();


            ViewBag.Medicines = new SelectList(
                medicines,
                "MedicineID",
                "MedicineName"
            );


            ViewBag.Equipment = new SelectList(
                equipment,
                "EquipmentID",
                "EquipmentName"
            );
        }


        // =========================================================
        // APPROVE REQUEST
        // =========================================================

        [Authorize(Roles = "Admin,Pharmacist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id)
        {
            var request = db.InventoryRequests
                            .FirstOrDefault(x => x.RequestID == id);

            if (request == null)
            {
                return HttpNotFound();
            }


            // -----------------------------------------------------
            // PREVENT APPROVING THE SAME REQUEST TWICE
            // -----------------------------------------------------

            if (request.Status == "Approved")
            {
                TempData["InventoryMessage"] =
                    "This request has already been approved.";

                return RedirectToAction("Index");
            }


            // -----------------------------------------------------
            // ONLY PENDING REQUESTS CAN BE APPROVED
            // -----------------------------------------------------

            if (request.Status != "Pending")
            {
                TempData["InventoryMessage"] =
                    "Only pending requests can be approved.";

                return RedirectToAction("Index");
            }


            // -----------------------------------------------------
            // FIND ITEM IN DATABASE
            // -----------------------------------------------------

            string itemName = request.ItemName.Trim();

            var medicine = db.Medicines
                .FirstOrDefault(x =>
                    x.MedicineName == itemName);


            // =====================================================
            // MEDICINE
            // =====================================================

            if (medicine != null)
            {
                // Re-check current stock
                if (medicine.Quantity < request.QuantityRequested)
                {
                    TempData["InventoryError"] =
                        "Cannot approve request. " +
                        "Only " +
                        medicine.Quantity +
                        " unit(s) of " +
                        medicine.MedicineName +
                        " are currently available.";

                    return RedirectToAction("Index");
                }


                // -------------------------------------------------
                // DEDUCT STOCK
                // -------------------------------------------------

                medicine.Quantity =
                    medicine.Quantity -
                    request.QuantityRequested;


                request.Status = "Approved";

                request.ApprovalNotes =
                    "Approved. " +
                    request.QuantityRequested +
                    " unit(s) deducted from medicine inventory.";


                db.SaveChanges();


                TempData["InventoryMessage"] =
                    "Request approved and medicine stock updated.";

                return RedirectToAction("Index");
            }


            // =====================================================
            // EQUIPMENT
            // =====================================================

            var equipment = db.Equipments
                .FirstOrDefault(x =>
                    x.EquipmentName == itemName);


            if (equipment != null)
            {
                // Re-check current quantity
                if (equipment.Quantity < request.QuantityRequested)
                {
                    TempData["InventoryError"] =
                        "Cannot approve request. " +
                        "Only " +
                        equipment.Quantity +
                        " unit(s) of " +
                        equipment.EquipmentName +
                        " are currently available.";

                    return RedirectToAction("Index");
                }


                // -------------------------------------------------
                // DEDUCT EQUIPMENT QUANTITY
                // -------------------------------------------------

                equipment.Quantity =
                    equipment.Quantity -
                    request.QuantityRequested;


                request.Status = "Approved";

                request.ApprovalNotes =
                    "Approved. " +
                    request.QuantityRequested +
                    " unit(s) deducted from equipment inventory.";


                db.SaveChanges();


                TempData["InventoryMessage"] =
                    "Request approved and equipment quantity updated.";

                return RedirectToAction("Index");
            }


            // -----------------------------------------------------
            // ITEM NO LONGER EXISTS
            // -----------------------------------------------------

            TempData["InventoryError"] =
                "Cannot approve request. The requested item no longer exists in inventory.";

            return RedirectToAction("Index");
        }


        // =========================================================
        // REJECT REQUEST
        // =========================================================

        [Authorize(Roles = "Admin,Pharmacist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int id)
        {
            var request =
                db.InventoryRequests.Find(id);

            if (request == null)
            {
                return HttpNotFound();
            }


            if (request.Status == "Approved")
            {
                TempData["InventoryError"] =
                    "An approved request cannot be rejected.";

                return RedirectToAction("Index");
            }


            request.Status = "Rejected";

            request.ApprovalNotes =
                "Request rejected by " +
                User.Identity.Name;


            db.SaveChanges();


            TempData["InventoryMessage"] =
                "Inventory request rejected.";

            return RedirectToAction("Index");
        }


        // =========================================================
        // EDIT - GET
        // =========================================================

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            InventoryRequest inventoryRequest =
                db.InventoryRequests.Find(id);

            if (inventoryRequest == null)
            {
                return HttpNotFound();
            }

            return View(inventoryRequest);
        }


        // =========================================================
        // EDIT - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(
                Include =
                "RequestID,ItemName,QuantityRequested,RequestedBy,RequestDate,Status,ApprovalNotes"
            )]
            InventoryRequest inventoryRequest
        )
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventoryRequest).State =
                    EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(inventoryRequest);
        }


        // =========================================================
        // DELETE - GET
        // =========================================================

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            InventoryRequest inventoryRequest =
                db.InventoryRequests.Find(id);

            if (inventoryRequest == null)
            {
                return HttpNotFound();
            }

            return View(inventoryRequest);
        }


        // =========================================================
        // DELETE - POST
        // =========================================================

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InventoryRequest inventoryRequest =
                db.InventoryRequests.Find(id);

            if (inventoryRequest != null)
            {
                db.InventoryRequests.Remove(
                    inventoryRequest
                );

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