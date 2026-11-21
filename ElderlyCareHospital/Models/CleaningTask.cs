using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareHospital.Models
{
    public class CleaningTask
    {
        [Key]
        public int TaskID { get; set; }

        public int? CleanerID { get; set; }

        public string Area { get; set; }

        public string Status { get; set; }

        public DateTime TaskDate { get; set; }

        [ForeignKey("CleanerID")]
        public virtual Cleaner Cleaner { get; set; }
    }
}
