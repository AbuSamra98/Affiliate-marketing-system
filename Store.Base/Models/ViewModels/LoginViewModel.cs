using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Store.Base.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression("^[A-Za-z0-9_@.-]*$", ErrorMessage = "Usernames should consist of the following characters only: a-z, A-Z, 0-9, -, _, @, .")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
