namespace ElderlyCareHospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class NurseNote
    {
        [Key]
        public int NoteID { get; set; }

        public string NoteText { get; set; }

        public DateTime DateCreated { get; set; }

        [ForeignKey("Patient")]
        public int PatientID { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
