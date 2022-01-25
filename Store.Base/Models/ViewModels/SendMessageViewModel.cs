using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Store.Base.Models.ViewModels
{
    public class SendMessageViewModel
    {
        [Required]
        public string Text { get; set; }

        public string UserName { get; set; }

        public int SendForSelect { get; set; }

        //public IList<ApplicationUser> Users { get; set; }

    }
}
