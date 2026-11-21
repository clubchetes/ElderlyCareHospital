using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareHospital.Models
{
    public class EmergencyAlert
    {
        [Key]
        public int AlertID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        public string AlertType { get; set; }

        public string Description { get; set; }

        public string Severity { get; set; }

        public DateTime AlertDate { get; set; }

        public string Status { get; set; }

        public string ResolvedBy { get; set; }

        public DateTime? ResolvedDate { get; set; }

        [ForeignKey("PatientID")]
        public virtual Patient Patient { get; set; }
    }
}