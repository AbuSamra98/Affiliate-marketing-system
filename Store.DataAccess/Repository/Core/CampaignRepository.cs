using Microsoft.EntityFrameworkCore;
using Store.Base.Models;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repository.Core
{
    public class CampaignRepository : Repository<Campaign>, ICampaignRepository
    {
        public CampaignRepository(ApplicationDbContext context) : base(context)
        {

        }
        public ApplicationDbContext _DBContext
        {
            get { return Context as ApplicationDbContext; }
        }


        public async Task<Campaign> FindCampaignIncludeType(int id)
        {
            return await Context.Set<Campaign>().Include(a => a.CampaignTypes).Where(x => x.Id == id).FirstOrDefaultAsync();
        }


        public async Task<Campaign> FindCampaignIncludeVendor(int id)
        {
            return await Context.Set<Campaign>().Include(a => a.Vendor.User).Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Campaign>> GetCampaigns()                     
        {
            return await Context.Set<Campaign>().Include(a => a.CampaignTypes).ToListAsync();
        }

        public async Task<IEnumerable<Campaign>> GetCampaigns(string VendorId)       
        {
            return await Context.Set<Campaign>().Where(x => x.VendorId == VendorId).Include(a => a.CampaignTypes).ToListAsync();
        }

        public async Task<IEnumerable<Campaign>> GetCampaignsIncludeVendor()                     
        {
            return await Context.Set<Campaign>().Include(a => a.CampaignTypes).Include(a => a.Vendor.User).ToListAsync();
        }

        public async Task<IEnumerable<Campaign>> GetApprovedCampaigns(bool isApproved)
        {
            return await Context.Set<Campaign>().Where(x => x.IsApproved == isApproved).Include(a => a.CampaignTypes).Include(a => a.Vendor.User).ToListAsync();
        }


        public async Task<IEnumerable<Campaign>> GetApprovedCampaigns(bool isApproved, bool isExpired)                    
        {
            if(isExpired==false)
            {
                return await Context.Set<Campaign>().Where(x => x.IsApproved == isApproved && x.ExpireDate > DateTime.Now).Include(a => a.CampaignTypes).Include(a => a.Vendor.User).ToListAsync();
            }
            else
            {
                return await Context.Set<Campaign>().Where(x => x.IsApproved == isApproved && x.ExpireDate < DateTime.Now).Include(a => a.CampaignTypes).Include(a => a.Vendor.User).ToListAsync();
            }
        }

        public async Task<IEnumerable<Campaign>> GetApprovedCampaigns(bool isApproved, bool isExpired, bool isEnabled)
        {
            if (isExpired == false)
            {
                return await Context.Set<Campaign>().Where(x => x.IsApproved == isApproved && x.Enable == isEnabled && x.ExpireDate > DateTime.Now).Include(a => a.CampaignTypes).Include(a => a.Vendor.User).ToListAsync();
            }
            else
            {
                return await Context.Set<Campaign>().Where(x => x.IsApproved == isApproved && x.Enable == isEnabled && x.ExpireDate < DateTime.Now).Include(a => a.CampaignTypes).Include(a => a.Vendor.User).ToListAsync();
            }
        }


        //public async Task<IEnumerable<Campaign>> GetSubscribedApprovedCampaigns(string marketer, bool isApproved, bool isExpired, bool isEnabled)
        //{

        //    if (isExpired == false)
        //    {
        //        return await Context.Set<Campaign>().Where(x => x.IsApproved == isApproved && x.Enable == isEnabled && x.ExpireDate > DateTime.Now).Include(a => a.CampaignTypes).Include(a => a.Vendor.User).ToListAsync();
        //    }
        //    else
        //    {
        //        return await Context.Set<Campaign>().Where(x => x.IsApproved == isApproved && x.Enable == isEnabled && x.ExpireDate < DateTime.Now).Include(a => a.CampaignTypes).Include(a => a.Vendor.User).ToListAsync();
        //    }
        //}

    }
}
