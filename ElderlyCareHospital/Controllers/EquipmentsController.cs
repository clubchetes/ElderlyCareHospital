using ElderlyCareHospital.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ElderlyCareHospital.Controllers
{
    public class EquipmentsController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // GET: Equipments
        public ActionResult Index()
        {
            return View(db.Equipments.ToList());
        }

        public ActionResult LowStock()
        {
            var equipment =
                db.Equipments
                .Where(x =>
                    x.Quantity <=
                    x.MinimumQuantity)
                .ToList();

            return View(equipment);
        }

        public ActionResult MaintenanceDue()
        {
            DateTime maintenanceDate =
                DateTime.Now.AddMonths(-6);

            var equipment =
                db.Equipments
                .Where(x =>
                    x.LastMaintenanceDate
                    <= maintenanceDate)
                .ToList();

            return View(equipment);
        }

        public class EquipmentAllocation
        {
            [Key]
            public int AllocationID
            {
                get;
                set;
            }

            public int EquipmentID
            {
                get;
                set;
            }

            public int PatientID
            {
                get;
                set;
            }

            public DateTime AllocatedDate
            {
                get;
                set;
            }

            public string Status
            {
                get;
                set;
            }

            public virtual Equipment Equipment
            {
                get;
                set;
            }

            public virtual Patient Patient
            {
                get;
                set;
            }

        }


        // GET: Equipments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipments.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // GET: Equipments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Equipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EquipmentID,EquipmentName,Category,Quantity,MinimumQuantity,Condition,PurchaseDate,LastMaintenanceDate,Location")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                db.Equipments.Add(equipment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(equipment);
        }

        // GET: Equipments/GetLiveEquipment
        public ActionResult GetLiveEquipment()
        {
            var equipmentList = db.Equipments.ToList();
            return PartialView("_EquipmentRows", equipmentList);
        }

        // GET: Equipments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipments.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EquipmentID,EquipmentName,Category,Quantity,MinimumQuantity,Condition,PurchaseDate,LastMaintenanceDate,Location")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(equipment);
        }

        // GET: Equipments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipments.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipment equipment = db.Equipments.Find(id);
            db.Equipments.Remove(equipment);
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
