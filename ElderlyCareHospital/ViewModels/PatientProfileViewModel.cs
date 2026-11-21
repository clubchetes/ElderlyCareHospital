using ElderlyCareHospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElderlyCareHospital.ViewModels
{
    public class PatientProfileViewModel
    {
        public Patient Patient { get; set; }

        public List<VitalSign> VitalSigns { get; set; }

        public List<DoctorNotes> DoctorNotes { get; set; }

        public List<NurseNote> NurseNotes { get; set; }

        public List<MedicationAdministration> Medications { get; set; }

        public CarePlan CarePlan { get; set; }
    }
}