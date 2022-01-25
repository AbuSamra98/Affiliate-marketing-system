using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Store.Base.Models.ViewModels
{
    public class AdminRegisterViewModel
    {

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z0-9_@.-]*$", ErrorMessage = "Usernames should consist of the following characters only: a-z, A-Z, 0-9, -, _, @, .")]
        public string Username { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z ]*$", ErrorMessage = "only letters")]
        public string FullName { get; set; }

    }
}
