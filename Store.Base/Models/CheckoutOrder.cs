using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Base.Models
{
    public class CheckoutOrder
    {
        public int Id { get; set; }

        public string VendorId { get; set; }

        [ForeignKey("VendorId")]
        public virtual Vendor Vendor { get; set; }

        public string OrderId { get; set; }

        public float Total { get; set; }

        public float PointValue { get; set; }

        public int AddedPoints { get; set; }

        public DateTime CreateDate { get; set; }


    }
}
