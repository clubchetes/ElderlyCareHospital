using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ElderlyCareHospital.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateOfBirth { get; set; }

        public string RoleName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<ApplicationUser> manager)
        {
            var userIdentity =
                await manager.CreateIdentityAsync(
                    this,
                    DefaultAuthenticationTypes.ApplicationCookie);

            return userIdentity;
        }
    }

    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection",
                   throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<DoctorNotes> DoctorNotes { get; set; }

        public DbSet<VitalSign> VitalSigns { get; set; }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Pharmacist> Pharmacists { get; set; }
        public DbSet<Cleaner> Cleaners { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
    public DbSet<CarePlan> CarePlans { get; set; }
        public DbSet<EmergencyAlert> EmergencyAlerts { get; set; }

        public DbSet<MedicationAdministration>
            MedicationAdministrations
        {
            get;
            set;
        }

        public DbSet<Medicine> Medicines { get; set; }
    }
}