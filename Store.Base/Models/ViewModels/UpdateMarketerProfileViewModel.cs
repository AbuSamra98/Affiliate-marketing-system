using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Store.Base.Models.ViewModels
{
    public class UpdateMarketerProfileViewModel
    {

        [Required]
        [RegularExpression("^[A-Za-z ]*$", ErrorMessage = "only letters")]
        public string FullName { get; set; }

        public int CountryId { get; set; }

        public IEnumerable<Country> countries { get; set; }


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
    }
}
