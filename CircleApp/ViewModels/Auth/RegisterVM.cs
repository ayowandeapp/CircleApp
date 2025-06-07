using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.ViewModels.Auth
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(250, MinimumLength = 2, ErrorMessage = "First name must be between 2 or 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage =" First name must contain only letters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage ="Last Name is required")]
        [StringLength(250, MinimumLength = 2, ErrorMessage = "Last name must be between 2 or 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage =" Last name must contain only letters")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage ="Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage ="Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}