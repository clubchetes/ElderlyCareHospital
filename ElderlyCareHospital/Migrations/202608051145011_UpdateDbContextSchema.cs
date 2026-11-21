namespace ElderlyCareHospital.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDbContextSchema : DbMigration
    {
        public override void Up()
        {
           
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pharmacists", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Nurses", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmergencyAlerts", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.Doctors", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Cleaners", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.FamilyMembers", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Pharmacists", new[] { "UserID" });
            DropIndex("dbo.Nurses", new[] { "UserID" });
            DropIndex("dbo.EmergencyAlerts", new[] { "PatientID" });
            DropIndex("dbo.Doctors", new[] { "UserID" });
            DropIndex("dbo.Cleaners", new[] { "UserID" });
            DropIndex("dbo.FamilyMembers", new[] { "UserID" });
            DropColumn("dbo.FamilyMembers", "UserID");
            DropTable("dbo.Pharmacists");
            DropTable("dbo.Nurses");
            DropTable("dbo.EmergencyAlerts");
            DropTable("dbo.Doctors");
            DropTable("dbo.Cleaners");
        }
    }
}
