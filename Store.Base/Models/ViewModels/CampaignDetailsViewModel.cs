using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Base.Models.ViewModels
{
    public class CampaignDetailsViewModel
    {
        public Campaign Campaign { get; set; }
        public IEnumerable<Campaign> RelatedCampaigns { get; set; }
        public string Owner { get; set; }
    }
}
