using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Base.Models
{
    public class Marketer
    {
        [Key]
        public string MarketerId { get; set; }

        [ForeignKey("MarketerId")]
        public virtual ApplicationUser User { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "only numbers")]
        public int Points { get; set; } = 0;

        public int CountryId { get; set; }

        [Required]
        [ForeignKey("CountryId")]
        [Display(Name = "Country")]
        public virtual Country Country { get; set; }


        [Required]
        [Display(Name = " Your website URL")]
        [Url]
        public string WebsiteURL { get; set; }


        [Display(Name = " Your Facebook URL")]
        [Url]
        public string Facebook { get; set; }

        [Display(Name = " Your Twitter URL")]
        [Url]
        public string Twitter { get; set; }

        [Display(Name = " Your Instagram URL")]
        [Url]
        public string Instagram { get; set; }

        [Display(Name = " Your YouTube URL")]
        [Url]
        public string YouTube { get; set; }

        [Url]
        public string Other { get; set; }

        [Display(Name = "Salary (USD)")]
        public float Salary { get; set; }

    }
}
