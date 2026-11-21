using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElderlyCareHospital.Models
{
    public class FamilyMember
    {
        [Key] public int FamilyMemberID { get; set; }
        public string UserID { get; set; }
        public int PatientID { get; set; }
        [Required] public string FullName { get; set; }
        public string Relationship { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [ForeignKey("UserID")] public virtual ApplicationUser User { get; set; }
        [ForeignKey("PatientID")] public virtual Patient Patient { get; set; }
    }
}