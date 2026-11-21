using ElderlyCareHospital.Models;
using ElderlyCareHospital.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;



public class UsersController : Controller
{
    private ApplicationDbContext identityDb = new ApplicationDbContext();
    private MyDatabaseModels db = new MyDatabaseModels();

    private UserManager<ApplicationUser> userManager;

    public UsersController()
    {
        userManager =
            new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(db));
    }
    public ActionResult Index(string searchString, string roleFilter)
    {
        var users = identityDb.Users.ToList();

        // Search by email or username
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            users = users.Where(u =>
                (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(searchString)) ||
                (!string.IsNullOrEmpty(u.UserName) && u.UserName.Contains(searchString))
            ).ToList();
        }

        // Get role names for each user
        var roleNames = new Dictionary<string, string>();

        foreach (var user in users)
        {
            var roleId = user.Roles.FirstOrDefault()?.RoleId;

            if (!string.IsNullOrEmpty(roleId))
            {
                var role = identityDb.Roles.Find(roleId);

                if (role != null)
                {
                    roleNames[user.Id] = role.Name;
                }
            }
        }

        // Filter by role
        if (!string.IsNullOrWhiteSpace(roleFilter))
        {
            users = users.Where(u =>
                roleNames.ContainsKey(u.Id) &&
                roleNames[u.Id] == roleFilter
            ).ToList();
        }

        // Send role names to the view
        ViewBag.UserRoles = roleNames;

        return View(users);
    }

    // GET: Users/GetLiveUsers
    public ActionResult GetLiveUsers(string searchString, string roleFilter)
    {
        var users = identityDb.Users.ToList();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            users = users.Where(u =>
                (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(searchString)) ||
                (!string.IsNullOrEmpty(u.UserName) && u.UserName.Contains(searchString))
            ).ToList();
        }

        var roleNames = new Dictionary<string, string>();
        foreach (var user in users)
        {
            var roleId = user.Roles.FirstOrDefault()?.RoleId;
            if (!string.IsNullOrEmpty(roleId))
            {
                var role = identityDb.Roles.Find(roleId);
                if (role != null)
                {
                    roleNames[user.Id] = role.Name;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(roleFilter))
        {
            users = users.Where(u =>
                roleNames.ContainsKey(u.Id) &&
                roleNames[u.Id] == roleFilter
            ).ToList();
        }

        ViewBag.UserRoles = roleNames;
        return PartialView("_UserRows", users);
    }
    public ActionResult Create()
    {
        ViewBag.RoleName =
            new SelectList(
                db.Roles.ToList(),
                "RoleName",
                "RoleName");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            // =========================
            // CREATE ASP.NET IDENTITY USER
            // =========================

            ApplicationUser identityUser =
                new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    RoleName = model.RoleName
                };

            var result =
                userManager.Create(
                    identityUser,
                    model.Password);

            if (result.Succeeded)
            {
                // Add role
                userManager.AddToRole(
                    identityUser.Id,
                    model.RoleName);

                // =========================
                // ALSO SAVE TO YOUR CUSTOM USERS TABLE
                // =========================

                User customUser =
                    new User
                    {
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        DateOfBirth = model.DateOfBirth,
                        PasswordHarsh = model.Password,
                        RoleName = model.RoleName
                    };

                db.Users.Add(customUser);
                db.SaveChanges();

                string fullName =
                    model.FirstName + " " + model.LastName;

                // =========================
                // DOCTOR
                // =========================

                if (model.RoleName == "Doctor")
                {
                    Doctor doctor = new Doctor
                    {
                        UserID = identityUser.Id, // STRING
                        FullName = fullName,
                        Speciality = model.Speciality,
                        Qualification = model.Qualification,
                        OfficeNumber = model.OfficeNumber,
                        PhoneNumber = model.PhoneNumber
                    };

                    identityDb.Doctors.Add(doctor);
                }

                // =========================
                // NURSE
                // =========================

                else if (model.RoleName == "Nurse")
                {
                    Nurse nurse = new Nurse
                    {
                        UserID = identityUser.Id, // STRING
                        FullName = fullName,
                        WardAssigned = model.WardAssigned,
                        ShiftType = model.ShiftType,
                        PhoneNumber = model.PhoneNumber
                    };

                    identityDb.Nurses.Add(nurse);
                }

                // =========================
                // PHARMACIST
                // =========================

                else if (model.RoleName == "Pharmacist")
                {
                    Pharmacist pharmacist = new Pharmacist
                    {
                        UserID = identityUser.Id, // STRING
                        FullName = fullName,
                        LicenseNumber = model.LicenseNumber,
                        PhoneNumber = model.PhoneNumber
                    };

                    identityDb.Pharmacists.Add(pharmacist);
                }

                // =========================
                // CLEANER
                // =========================

                else if (model.RoleName == "Cleaner")
                {
                    Cleaner cleaner = new Cleaner
                    {
                        UserID = identityUser.Id, // STRING
                        FullName = fullName,
                        AreaAssigned = model.AreaAssigned,
                        ShiftType = model.ShiftType,
                        PhoneNumber = model.PhoneNumber
                    };

                    identityDb.Cleaners.Add(cleaner);
                }

                identityDb.SaveChanges();

                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        ViewBag.RoleName =
            new SelectList(
                db.Roles.ToList(),
                "RoleName",
                "RoleName",
                model.RoleName);

        return View(model);
    }
    public ActionResult Edit(int id)
{

        // 2. Query your actual database table instead of userManager
        var user = db.Users.FirstOrDefault(u => u.UserID == id);

        if (user == null)
        {
            return HttpNotFound();
        }

       

        EditUserViewModel vm = new EditUserViewModel
        {
            Id = user.UserID.ToString(), // This red underline will completely disappear now!

            FirstName =
                user.FirstName,

            LastName =
                user.LastName,

            Email =
                user.Email,

            DateOfBirth =
                user.DateOfBirth,

            RoleName = 
                user.RoleName

          
        };

    ViewBag.RoleName =
        new SelectList(
            db.Roles.ToList(),
            "RoleName",
            "RoleName",
            vm.RoleName
            
            );

    return View(vm);
}


    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditUserViewModel vm)
    {
        if (ModelState.IsValid)
        {
            // 1. Convert ViewModel string Id to integer to match UserID column
            int userIdInt = System.Convert.ToInt32(vm.Id);

            // 2. Query your actual database table instead of userManager
            var user = db.Users.FirstOrDefault(u => u.UserID == userIdInt);

            if (user == null)
            {
                return HttpNotFound();
            }

            // 3. Map form fields to the database entity properties
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Email = vm.Email;
            user.RoleName = vm.RoleName;
          
            user.DateOfBirth = vm.DateOfBirth; // Directly assign since types match now

            // 4. Save directly back to SQL Server
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // If something goes wrong, repopulate the dropdown selection list and return
        ViewBag.RoleList = new SelectList(db.Roles.ToList(), "RoleName", "RoleName", vm.RoleName);
        return View(vm);
    }

    // 1. GET: Users/Delete/5
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }

        // Look up the user directly out of your custom table using the integer ID
        var user = db.Users.FirstOrDefault(u => u.UserID == id);

        if (user == null)
        {
            return HttpNotFound();
        }

        return View(user);
    }

    // 2. POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        // Find the user directly via your db context 
        var user = db.Users.FirstOrDefault(u => u.UserID == id);

        if (user != null)
        {
            db.Users.Remove(user);
            db.SaveChanges(); // Persist the deletion directly to SQL Server
        }

        return RedirectToAction("Index");
    }
}



