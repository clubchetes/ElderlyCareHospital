using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElderlyCareHospital.Models
{
    public class EncouragementMessage
    {
        [Key]
        public int MessageID { get; set; }

        public int PatientID { get; set; }

        public int FamilyID { get; set; }

        public string MessageText { get; set; }

        public DateTime DateSent { get; set; }
    }
}