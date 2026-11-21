namespace ElderlyCareHospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Visit
    {
        public int VisitID { get; set; }

        public int? FamilyID { get; set; }

        public DateTime? VisitDate { get; set; }

        [StringLength(50)]
        public string ParkingSlot { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
    }
}
