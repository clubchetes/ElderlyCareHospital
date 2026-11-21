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
    public class CleaningTasksController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // GET: CleaningTasks
        public ActionResult Index()
        {
            var tasks =
                db.CleaningTasks
                  .Include(c => c.Cleaner)
                  .ToList();

            return View(tasks);
        }
        // GET: CleaningTasks/GetLiveTasks
        public ActionResult GetLiveTasks()
        {
            var tasks = db.CleaningTasks.Include(c => c.Cleaner).ToList();
            return PartialView("_CleaningTaskRows", tasks);
        }

        // GET: CleaningTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CleaningTask cleaningTask = db.CleaningTasks.Find(id);
            if (cleaningTask == null)
            {
                return HttpNotFound();
            }
            return View(cleaningTask);
        }

        // GET: CleaningTasks/Create
        // GET: CleaningTasks/Create
        public ActionResult Create()
        {
            ViewBag.CleanerID =
                new SelectList(
                    db.Cleaners.ToList(),
                    "CleanerID",
                    "FullName");

            return View();
        }

        // POST: CleaningTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
      [Bind(Include =
        "TaskID,CleanerID,Area,Status,TaskDate")]
    CleaningTask cleaningTask)
        {
            if (ModelState.IsValid)
            {
                db.CleaningTasks.Add(cleaningTask);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Reload dropdown if validation fails
            ViewBag.CleanerID =
                new SelectList(
                    db.Cleaners.ToList(),
                    "CleanerID",
                    "FullName",
                    cleaningTask.CleanerID);

            return View(cleaningTask);
        }

        // GET: CleaningTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CleaningTask cleaningTask = db.CleaningTasks.Find(id);
            if (cleaningTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.CleanerID =
      new SelectList(
          db.Cleaners.ToList(),
          "CleanerID",
          "FullName",
          cleaningTask.CleanerID);

            return View(cleaningTask);
        }

        // POST: CleaningTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
     [Bind(Include =
        "TaskID,CleanerID,Area,Status,TaskDate")]
    CleaningTask cleaningTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cleaningTask).State =
                    EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CleanerID =
                new SelectList(
                    db.Cleaners.ToList(),
                    "CleanerID",
                    "FullName",
                    cleaningTask.CleanerID);

            return View(cleaningTask);
        }

        // GET: CleaningTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CleaningTask cleaningTask = db.CleaningTasks.Find(id);
            if (cleaningTask == null)
            {
                return HttpNotFound();
            }
            return View(cleaningTask);
        }

        // POST: CleaningTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CleaningTask cleaningTask = db.CleaningTasks.Find(id);
            db.CleaningTasks.Remove(cleaningTask);
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
