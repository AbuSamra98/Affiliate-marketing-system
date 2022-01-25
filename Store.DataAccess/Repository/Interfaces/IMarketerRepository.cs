using Store.Base.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repository.Interfaces
{
    public interface IMarketerRepository : IRepository<Marketer>
    {
        Task<Marketer> FindMarketer(string id);
        Task<IEnumerable<Marketer>> GetAllMarketers();
    }
}
