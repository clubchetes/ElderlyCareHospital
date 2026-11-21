using System;
using System.ComponentModel.DataAnnotations;

namespace ElderlyCareHospital.ViewModels
{
    public class CreateUserViewModel
    {
        // =========================
        // BASIC USER INFO
        // =========================

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string RoleName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // =========================
        // SHARED
        // =========================

        public string PhoneNumber { get; set; }

        // =========================
        // DOCTOR
        // =========================

        public string Speciality { get; set; }

        public string Qualification { get; set; }

        public string OfficeNumber { get; set; }

        // =========================
        // NURSE
        // =========================

        public string WardAssigned { get; set; }

        public string ShiftType { get; set; }

        // =========================
        // PHARMACIST
        // =========================

        public string LicenseNumber { get; set; }

        // =========================
        // CLEANER
        // =========================

        public string AreaAssigned { get; set; }
    }
}