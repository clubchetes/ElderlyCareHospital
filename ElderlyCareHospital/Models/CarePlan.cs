namespace ElderlyCareHospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CarePlan
    {
        [Key]
        public int CarePlanID { get; set; }

        public int? PatientID { get; set; }

        public string MedicationRoutine { get; set; }

        public string DietInstructions { get; set; }

        public string MobilitySupport { get; set; }

        public string TherapySchedule { get; set; }

        public string MemoryCareNotes { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
