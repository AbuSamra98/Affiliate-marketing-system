using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Store.Base.Models
{
    public class CampaignTypes
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z]*$", ErrorMessage = "only letters")]
        [MaxLength(20, ErrorMessage = "Length more than 20 chars")]
        [Display(Name = "Campaign Type")]
        public string Name { get; set; }
    }
}
