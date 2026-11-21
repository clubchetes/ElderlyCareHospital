namespace ElderlyCareHospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MedicationAdministration")]
    public partial class MedicationAdministration
    {
        [Key]
        public int AdministrationID { get; set; }

        public int? PatientID { get; set; }

        public int? MedicineID { get; set; }

        public int? NurseID { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public DateTime? DateAdministered { get; set; }

        public virtual Medicine Medicine { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
