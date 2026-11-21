using ElderlyCareHospital.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

// This assembly attribute is the "Magic Hook" that tells IIS 
// exactly which class to run when the application starts.
[assembly: OwinStartup(typeof(ElderlyCareHospital.Startup))]

namespace ElderlyCareHospital
{
    public partial class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            // This calls the ConfigureAuth method defined in your Startup.Auth.cs
            ConfigureAuth(app);
            var roleManager =
new RoleManager<IdentityRole>(
new RoleStore<IdentityRole>(
new ApplicationDbContext()));

            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(
                    new IdentityRole("Admin"));
            }

            if (!roleManager.RoleExists("Doctor"))
            {
                roleManager.Create(
                    new IdentityRole("Doctor"));
            }

            if (!roleManager.RoleExists("Nurse"))
            {
                roleManager.Create(
                    new IdentityRole("Nurse"));
            }

            if (!roleManager.RoleExists("Pharmacist"))
            {
                roleManager.Create(
                    new IdentityRole("Pharmacist"));
            }

            if (!roleManager.RoleExists("Cleaner"))
            {
                roleManager.Create(
                    new IdentityRole("Cleaner"));
            }

            if (!roleManager.RoleExists("FamilyMember"))
            {
                roleManager.Create(
                    new IdentityRole("FamilyMember"));
            }
        }
    }

}