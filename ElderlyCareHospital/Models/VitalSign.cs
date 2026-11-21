namespace ElderlyCareHospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class VitalSign
    {
        [Key]
        public int VitalID { get; set; }

        public decimal Temperature { get; set; }

        public string BloodPressure { get; set; }

        public int PulseRate { get; set; }

        public int OxygenLevel { get; set; }

        public decimal SugarLevel { get; set; }

        public decimal Weight { get; set; }

        public DateTime DateRecorded { get; set; }

        [ForeignKey("Patient")]
        public int PatientID { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
