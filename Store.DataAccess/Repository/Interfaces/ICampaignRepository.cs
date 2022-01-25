using Store.Base.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repository.Interfaces
{
    public interface ICampaignRepository : IRepository<Campaign>
    {
        Task<Campaign> FindCampaignIncludeType(int id);

        //get campaign include vendor
        Task<Campaign> FindCampaignIncludeVendor(int id);

        //get all Campaign
        Task<IEnumerable<Campaign>> GetCampaigns();

        //get Campaigns for specific vendor
        Task<IEnumerable<Campaign>> GetCampaigns(string VendorId);

        //get all Campaign include vendor of every Campaign
        Task<IEnumerable<Campaign>> GetCampaignsIncludeVendor();

        //[isApproved] >> get approved Campaigns (true) not approved (false)
        Task<IEnumerable<Campaign>> GetApprovedCampaigns(bool isApproved);

        //[isApproved] >> get approved Campaign (true) not approved (false)
        //[isExpired] >> get expired Campaign (true) not expired (false)
        Task<IEnumerable<Campaign>> GetApprovedCampaigns(bool isApproved, bool isExpired);

        //[isApproved] >> get approved Campaign (true) not approved (false)
        //[isExpired] >> get expired Campaign (true) not expired (false)
        //[isEnabled] >> get enabled Campaign (true) not enabled (false)
        Task<IEnumerable<Campaign>> GetApprovedCampaigns(bool isApproved, bool isExpired, bool isEnabled);
    }
}
