using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Base.Models
{
    public class AboutUs
    {
        public int Id { get; set; }

        [Column(TypeName = "Text")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
