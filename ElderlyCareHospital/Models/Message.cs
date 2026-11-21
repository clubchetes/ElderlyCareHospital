namespace ElderlyCareHospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Message
    {
        [Key]
        public int MessageID { get; set; }

        public int? SenderID { get; set; }

        public int? ReceiverID { get; set; }

        public string MessageText { get; set; }

        public DateTime? SentDate { get; set; }
    }
}
