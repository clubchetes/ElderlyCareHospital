using System;
using System.ComponentModel.DataAnnotations;

namespace ElderlyCareHospital.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentID { get; set; }

        [Required]
        public string EquipmentName { get; set; }

        public string Category { get; set; }

        public int Quantity { get; set; }

        public int MinimumQuantity { get; set; }

        public string Condition
        {
            get;
            set;
        }

        public DateTime PurchaseDate
        {
            get;
            set;
        }

        public DateTime LastMaintenanceDate
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }
    }
}