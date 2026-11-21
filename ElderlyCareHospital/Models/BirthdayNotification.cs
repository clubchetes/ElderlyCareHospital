namespace ElderlyCareHospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BirthdayNotification
    {
        [Key]
        public int BirthdayID { get; set; }

        public int? UserID { get; set; }

        public DateTime? NotificationDate { get; set; }
    }
}
