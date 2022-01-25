using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Base.Models
{
    public class UpdateVendorProfileRequest
    {
        [Key]
        [ForeignKey("User")]
        public string Id { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


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

        public virtual Country country { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "only numbers")]
        public int ZIP { get; set; }
    }
}
