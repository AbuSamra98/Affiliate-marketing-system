using Microsoft.EntityFrameworkCore;
using Store.Base.Models;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;


namespace Store.DataAccess.Repository.Core
{
    public class VendorRepository : Repository<Vendor>, IVendorRepository
    {
        public VendorRepository(ApplicationDbContext context) : base(context)
        {

        }
        public ApplicationDbContext _DBContext
        {
            get { return Context as ApplicationDbContext; }
        }


        public async Task<IEnumerable<Vendor>> GetAllVendors()
        {
            return await Context.Set<Vendor>().Include(a => a.User).ToListAsync();
        }


        public async Task<IEnumerable<Vendor>> GetNotApprovedVendors()
        {
            return await Context.Set<Vendor>().Where(x => x.User.LockoutEnd == null || x.User.LockoutEnd > DateTime.Now).Include(a => a.User).ToListAsync();
        }


        public async Task<Vendor> FindVendor(string id)
        {
            return await Context.Set<Vendor>().Include(x => x.User).SingleOrDefaultAsync(m => m.VendorId == id);
        }



        //public async Task<IEnumerable<Vendor>> GetNotApprovedVendors()
        //{
        //    return await Context.Set<Vendor>().Include(a => a.Trainers).ToListAsync();
        //}
    }
}
