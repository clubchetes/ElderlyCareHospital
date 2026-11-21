using ElderlyCareHospital.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

[Authorize(Roles = "FamilyMember")]
public class FamilyController : Controller
{
    private MyDatabaseModels db = new MyDatabaseModels();


    public ActionResult Dashboard()
    {
        string userId = User.Identity.GetUserId();

        var family = db.FamilyMembers
                       .FirstOrDefault(x => x.UserID == userId);

        if (family == null)
            return HttpNotFound();

        var patient = db.Patients.Find(family.PatientID);

        // Count visits for this family member
        ViewBag.Visits = db.Visits.Count(v => v.FamilyID == family.FamilyMemberID);

        ViewBag.Messages = db.Messages
                            .Count(m => m.ReceiverID == family.FamilyMemberID);

        return View(patient);
    }
    [Authorize(Roles = "FamilyMember")]
    public ActionResult GetFamilyDashboardJson()
    {
        string userId = User.Identity.GetUserId();

        var family = db.FamilyMembers
                       .FirstOrDefault(x => x.UserID == userId);

        if (family == null)
            return HttpNotFound();

        var patient = db.Patients.Find(family.PatientID);

        ViewBag.Visits = db.Visits.Count(v => v.FamilyID == family.FamilyMemberID);
        ViewBag.Messages = db.Messages.Count(m => m.ReceiverID == family.FamilyMemberID);

        return PartialView("_FamilyDetails", patient);
    }
}