using ElderlyCareHospital.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace ElderlyCareHospital.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private ApplicationDbContext db =
            new ApplicationDbContext();

        private MyDatabaseModels dbb = new MyDatabaseModels();

        public ActionResult Dashboard()
        {
            string userId =
                User.Identity.GetUserId();

            var doctor =
                db.Doctors
                  .FirstOrDefault(x =>
                      x.UserID == userId);

            if (doctor == null)
            {
                ViewBag.DoctorName =
                    User.Identity.Name;

                ViewBag.Speciality =
                    "General Doctor";
            }
            else
            {
                ViewBag.DoctorName =
                    doctor.FullName;

                ViewBag.Speciality =
                    doctor.Speciality;
            }

            ViewBag.TotalPatients =
                db.Patients.Count();

            ViewBag.CarePlans =
                db.CarePlans.Count();

            ViewBag.ActiveAlerts =
                db.EmergencyAlerts
                  .Count(x =>
                      x.Status == "Active");

            ViewBag.Messages = dbb.Messages
                           .Count(m => m.ReceiverID == doctor.DoctorID);

            return View();
        }
        [Authorize(Roles = "Doctor")]
        public ActionResult GetDoctorStatsJson()
        {
            string userId = User.Identity.GetUserId();
            var doctor = db.Doctors.FirstOrDefault(x => x.UserID == userId);
            int doctorId = doctor != null ? doctor.DoctorID : 0;

            ViewBag.TotalPatients = db.Patients.Count();
            ViewBag.CarePlans = db.CarePlans.Count();
            ViewBag.ActiveAlerts = db.EmergencyAlerts.Count(x => x.Status == "Active");
            ViewBag.Messages = dbb.Messages.Count(m => m.ReceiverID == doctorId);

            return PartialView("_DoctorStats");
        }
    }
}