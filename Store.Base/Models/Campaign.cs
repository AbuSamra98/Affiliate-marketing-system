using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Base.Models
{
    public class Campaign
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "Length more than 20 chars")]
        public string Title { get; set; }

        [Required]
        public string Summary { get; set; }

        [Required]
        [Column(TypeName = "Text")]
        [Display(Name = "Description")]
        public string Desc { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "only numbers")]
        public int Points { get; set; }

        [Required]
        [Display(Name = "Product URL")]
        [Url]
        public string URL { get; set; }

        [Required]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Expire Date")]
        public DateTime ExpireDate { get; set; }

        [Display(Name = "Expire Time")]
        [NotMapped]     
        public DateTime ExpireTime { get; set; }

        public string Image { get; set; }


        public int CampaignTypeId { get; set; } = 1;

        [ForeignKey("CampaignTypeId")]
        [Display(Name = "Campaign Type")]
        public virtual CampaignTypes CampaignTypes { get; set; }


        public string VendorId { get; set; }

        [ForeignKey("VendorId")]
        public virtual Vendor Vendor { get; set; }

        [Display(Name = "Approval")]
        public bool IsApproved { get; set; } = false;

        [Display(Name = " Disable/Enable")]
        public bool Enable { get; set; } = true;

        [Required]
        [Display(Name = "Number of subscribers")]
        public int SubscribersCount { get; set; } = 0;

    }
}
