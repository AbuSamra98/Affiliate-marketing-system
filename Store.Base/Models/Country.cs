using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Store.Base.Models
{
    public class Country
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z]*$", ErrorMessage = "only letters")]
        [MaxLength(30, ErrorMessage = "Length more than 30 chars")]
        [Display(Name = "Country")]
        public string Name { get; set; }
    }
}
