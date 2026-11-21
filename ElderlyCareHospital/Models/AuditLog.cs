using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElderlyCareHospital.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditID { get; set; }

        public string UserName { get; set; }

        public string ActionPerformed { get; set; }

        public DateTime ActionDate { get; set; }
    }
}