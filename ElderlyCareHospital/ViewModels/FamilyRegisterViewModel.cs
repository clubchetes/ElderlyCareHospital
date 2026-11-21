using System;
using System.ComponentModel.DataAnnotations;

namespace ElderlyCareHospital.ViewModels
{
    public class FamilyRegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(200, ErrorMessage = "Email address cannot exceed 200 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(?:\+27|0)[6-8][0-9]{8}$",
            ErrorMessage = "Please enter a valid South African phone number.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Relationship is required.")]
        [StringLength(50, ErrorMessage = "Relationship cannot exceed 50 characters.")]
        public string Relationship { get; set; }

        [Required(ErrorMessage = "Patient first name is required.")]
        [StringLength(100, ErrorMessage = "Patient first name cannot exceed 100 characters.")]
        public string PatientFirstName { get; set; }

        [Required(ErrorMessage = "Patient last name is required.")]
        [StringLength(100, ErrorMessage = "Patient last name cannot exceed 100 characters.")]
        public string PatientLastName { get; set; }

        [Required(ErrorMessage = "Patient date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime PatientDateOfBirth { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}

