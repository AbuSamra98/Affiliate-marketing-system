using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Base.Models.ViewModels
{
    public class CampaignViewModel
    {
        public Campaign Campaign { get; set; }
        public IEnumerable<CampaignTypes> CampaignTypes { get; set; }

        public int SubscribersCount { get; set; }
    }
}
