using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Base.Models
{
    public class CompainsSubscribedByMarketer
    {
        public int Id { get; set; }
        public string MarketerId { get; set; }

        [ForeignKey("MarketerId")]
        public virtual Marketer Marketers { get; set; }


        public int CampaignId { get; set; }

        [ForeignKey("CampaignId")]
        public virtual Campaign Campaigns { get; set; }
    }
}
