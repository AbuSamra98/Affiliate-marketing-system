using Store.Base.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repository.Interfaces
{
    public interface IVendorRepository : IRepository<Vendor>
    {
        Task<IEnumerable<Vendor>> GetAllVendors();
        Task<IEnumerable<Vendor>> GetNotApprovedVendors();
        Task<Vendor> FindVendor(string id);
        //Task<IEnumerable<Vendor>> GetNotApprovedVendors();
    }
}
