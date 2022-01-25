using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Store.Base.Models
{
    public class PointSettings
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Point Value (USD)")]
        public float PointValue { get; set; }

        [Required]
        [Display(Name = "Percentage For Admin")]
        public float PercentageForAdmin { get; set; }

    }
}
