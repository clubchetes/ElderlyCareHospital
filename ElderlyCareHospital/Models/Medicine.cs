namespace ElderlyCareHospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Medicine
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Medicine()
        {
            MedicationAdministrations = new HashSet<MedicationAdministration>();
        }
        [Key]
        public int MedicineID { get; set; }

        [StringLength(200)]
        public string MedicineName { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }

        public int? Quantity { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }

        public int? MinimumStock { get; set; }

        public string Supplier { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedicationAdministration> MedicationAdministrations { get; set; }
    }
}
