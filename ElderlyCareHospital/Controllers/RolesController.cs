using ElderlyCareHospital.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Web.Mvc;


public class RolesController : Controller
{
    private ApplicationDbContext identityDb = new ApplicationDbContext();
    private MyDatabaseModels db = new MyDatabaseModels();

    private UserManager<ApplicationUser> roleManager;

    public RolesController()
    {
        roleManager =
            new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(db));
    }
    public ActionResult Index()
    {
        var roles = identityDb.Roles.ToList();
        return View(roles);
    }

    // GET: Roles/GetLiveRoles
    public ActionResult GetLiveRoles()
    {
        var roles = identityDb.Roles.ToList();
        return PartialView("_RoleRows", roles);
    }
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(
    Role role)
    {
        if (ModelState.IsValid)
        {
            db.Roles.Add(role);
            db.SaveChanges();

            return RedirectToAction(
                "Index");
        }

        return View(role);
    }

    public ActionResult Delete(string id)
    {
        var role =
            roleManager.FindById(id);

        return View(role);
    }


}
