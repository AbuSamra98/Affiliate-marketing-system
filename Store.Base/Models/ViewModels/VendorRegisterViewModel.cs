using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Store.Base.Models.ViewModels
{
    public class VendorRegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z0-9_@.-]*$", ErrorMessage = "Usernames should consist of the following characters only: a-z, A-Z, 0-9, -, _, @, .")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

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

        [Required]
        [Display(Name = "Organization and Company")]
        public string OrganizationAndCompany { get; set; }

        [Required]
        [Display(Name = " Your website URL")]
        [Url]
        public string WebsiteURL { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z]*$", ErrorMessage = "only letters")]
        public string City { get; set; }

        public int CountryId { get; set; }

        public IEnumerable<Country> countries { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "only numbers")]
        public int ZIP { get; set; }

    }
}
