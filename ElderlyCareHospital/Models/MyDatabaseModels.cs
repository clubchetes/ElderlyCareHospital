using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Razor.Generator;
using static ElderlyCareHospital.Controllers.EquipmentsController;
namespace ElderlyCareHospital.Models
{
    
        public partial class MyDatabaseModels : IdentityDbContext<ApplicationUser>
        {
        public MyDatabaseModels() : base("name=HospitalConnection")
        {
        }
        public static MyDatabaseModels Create()
        {
            return new MyDatabaseModels();
        }
        public virtual DbSet<BirthdayNotification> BirthdayNotifications { get; set; }
        public virtual DbSet<CarePlan> CarePlans { get; set; }
        public virtual DbSet<CleaningTask> CleaningTasks { get; set; }
        public virtual DbSet<DoctorNotes> DoctorNotes { get; set; }
        public DbSet<EmergencyAlert> EmergencyAlerts
        {
            get;
            set;
        }
        public virtual DbSet<Equipment> Equipments { get; set; }
        

        public virtual DbSet<MedicationAdministration> MedicationAdministrations { get; set; }
        public virtual DbSet<Medicine> Medicines { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<NurseNote> NurseNotes { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Visit> Visits { get; set; }
        public virtual DbSet<VitalSign> VitalSigns { get; set; }
        public  DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Pharmacist> Pharmacists { get; set; }
        public DbSet<Cleaner> Cleaners { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<InventoryRequest>
  InventoryRequests
        {
            get;
            set;
        }

        public DbSet<EquipmentAllocation>
EquipmentAllocations
        {
            get;
            set;
        }

        public void CreateNotification(
        int userId,
        string title,
        string message)
        {
            Notification notification =
                new Notification();

            notification.UserID = userId;
            notification.Title = title;
            notification.Message = message;
            notification.IsRead = false;
            notification.CreatedDate = DateTime.Now;

            this.Notifications.Add(notification);
            this.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CarePlan>()
                .Property(e => e.MedicationRoutine)
                .IsUnicode(false);

            modelBuilder.Entity<CarePlan>()
                .Property(e => e.DietInstructions)
                .IsUnicode(false);

            modelBuilder.Entity<CarePlan>()
                .Property(e => e.MobilitySupport)
                .IsUnicode(false);

            modelBuilder.Entity<CarePlan>()
                .Property(e => e.TherapySchedule)
                .IsUnicode(false);

            modelBuilder.Entity<CarePlan>()
                .Property(e => e.MemoryCareNotes)
                .IsUnicode(false);

            modelBuilder.Entity<CleaningTask>()
                .Property(e => e.Area)
                .IsUnicode(false);

            modelBuilder.Entity<CleaningTask>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<DoctorNotes>()
                .Property(e => e.NoteText)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencyAlert>()
                .Property(e => e.AlertType)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencyAlert>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencyAlert>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<Equipment>()
                .Property(e => e.EquipmentName)
                .IsUnicode(false);

            modelBuilder.Entity<FamilyMember>()
                .Property(e => e.FullName)
                .IsUnicode(false);

            modelBuilder.Entity<FamilyMember>()
                .Property(e => e.Relationship)
                .IsUnicode(false);

            modelBuilder.Entity<FamilyMember>()
                .Property(e => e.Email)
                .IsUnicode(false);

      
           

            modelBuilder.Entity<MedicationAdministration>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<Medicine>()
                .Property(e => e.MedicineName)
                .IsUnicode(false);

            modelBuilder.Entity<Message>()
                .Property(e => e.MessageText)
                .IsUnicode(false);

            modelBuilder.Entity<NurseNote>()
                .Property(e => e.NoteText)
                .IsUnicode(false);

            modelBuilder.Entity<Patient>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<Patient>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<Patient>()
                .Property(e => e.Gender)
                .IsUnicode(false);

            modelBuilder.Entity<Patient>()
                .Property(e => e.RoomNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Patient>()
                .Property(e => e.EmergencyContact)
                .IsUnicode(false);

            modelBuilder.Entity<Patient>()
                .Property(e => e.Allergies)
                .IsUnicode(false);

            modelBuilder.Entity<Patient>()
                .Property(e => e.MedicalHistory)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.RoleName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.PasswordHarsh)
                .IsUnicode(false);

            modelBuilder.Entity<Visit>()
                .Property(e => e.ParkingSlot)
                .IsUnicode(false);

            modelBuilder.Entity<Visit>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<VitalSign>()
                .Property(e => e.Temperature)
                .HasPrecision(5, 2);

            modelBuilder.Entity<VitalSign>()
                .Property(e => e.BloodPressure)
                .IsUnicode(false);

            modelBuilder.Entity<VitalSign>()
                .Property(e => e.SugarLevel)
                .HasPrecision(5, 2);

            modelBuilder.Entity<VitalSign>()
                .Property(e => e.Weight)
                .HasPrecision(5, 2);
        }

        public System.Data.Entity.DbSet<ElderlyCareHospital.Models.EditUserViewModel> EditUserViewModels { get; set; }

        public System.Data.Entity.DbSet<ElderlyCareHospital.Models.EncouragementMessage> EncouragementMessages { get; set; }
    }
}
