using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Base.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string ReceiverUserId { get; set; }

        [ForeignKey("ReceiverUserId")]
        public virtual ApplicationUser ReceiverUser { get; set; }

        [Required]
        public string Text { get; set; }
        
        public bool Read { get; set; } = false;

        public DateTime SendDate { get; set; } = DateTime.Now;
    }
}
