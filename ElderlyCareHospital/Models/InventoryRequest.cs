using System;
using System.ComponentModel.DataAnnotations;

namespace ElderlyCareHospital.Models
{
    public class InventoryRequest
    {
        [Key]
        public int RequestID { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        public int QuantityRequested { get; set; }

        public string RequestedBy { get; set; }

        public DateTime RequestDate { get; set; }

        public string Status { get; set; }

        public string ApprovalNotes { get; set; }
    }
}