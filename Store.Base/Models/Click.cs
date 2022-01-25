using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Base.Models
{
    public class Click
    {
        public int Id { get; set; }

        public int LinkId { get; set; }

        [ForeignKey("LinkId")]
        public virtual Link Link { get; set; }

        public string IPAddress { get; set; }

        public DateTime Date { get; set; }
    }
}
