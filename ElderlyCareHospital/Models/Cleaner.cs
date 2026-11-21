using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareHospital.Models
{
    public class Cleaner
    {
        [Key] public int CleanerID { get; set; }
        public string UserID { get; set; }
        [Required] public string FullName { get; set; }
        public string AreaAssigned { get; set; }
        public string ShiftType { get; set; }
        public string PhoneNumber { get; set; }
        [ForeignKey("UserID")] public virtual ApplicationUser User { get; set; }
    }
}