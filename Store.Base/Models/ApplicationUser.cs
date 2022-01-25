using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Store.Base.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [RegularExpression("^[A-Za-z]*$", ErrorMessage = "only letters")]
        public string FullName { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

    }
}
