using ElderlyCareHospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ElderlyCareHospital.Controllers
{
    [Authorize(Roles = "Cleaner")]
    public class CleanerController : Controller
    {

        private MyDatabaseModels db = new MyDatabaseModels();

        public ActionResult Dashboard()
        {
            // Logged-in ASP.NET Identity user ID
            string userId = User.Identity.GetUserId();

            // Find the cleaner record linked to this user
            var cleaner = db.Cleaners
                            .FirstOrDefault(c => c.UserID == userId);

            // If no cleaner record exists, show empty list
            if (cleaner == null)
            {
                return View(new List<CleaningTask>());
            }

            // Only show tasks assigned to this cleaner
            var tasks = db.CleaningTasks
                          .Where(t => t.CleanerID == cleaner.CleanerID)
                          .ToList();

            return View(tasks);
        }
        [Authorize(Roles = "Cleaner")]
        public ActionResult GetCleaningTasksJson()
        {
            string userId = User.Identity.GetUserId();
            var cleaner = db.Cleaners.FirstOrDefault(c => c.UserID == userId);

            if (cleaner == null)
            {
                return PartialView("_CleaningTasksTable", new List<CleaningTask>());
            }

            var tasks = db.CleaningTasks
                          .Where(t => t.CleanerID == cleaner.CleanerID)
                          .ToList();

            return PartialView("_CleaningTasksTable", tasks);
        }

        public ActionResult Complete(int id)
        {
            var task =
                db.CleaningTasks.Find(id);

            task.Status = "Completed";

            db.SaveChanges();

            return RedirectToAction("Dashboard");
        }
        [AllowAnonymous]
        public ActionResult Resolve(int id)
        {
            var alert =
                db.CleaningTasks
                  .Find(id);

            alert.Status =
                "Resolved";


            alert.TaskDate =
                DateTime.Now;

            db.SaveChanges();

            return RedirectToAction(
                "Dashboard");
        }
    }
}