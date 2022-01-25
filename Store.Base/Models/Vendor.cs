using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Store.Base.Models
{
    public class Vendor
    {
        [Key]
        public string VendorId { get; set; }

        [ForeignKey("VendorId")]
        public virtual ApplicationUser User { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "only numbers")]
        public int Points { get; set; } = 0;



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

        [Required]
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }


        [Required]
        public int ZIP { get; set; }




    }
}
