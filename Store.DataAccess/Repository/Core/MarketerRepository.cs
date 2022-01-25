using Microsoft.EntityFrameworkCore;
using Store.Base.Models;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repository.Core
{
    public class MarketerRepository : Repository<Marketer>, IMarketerRepository
    {
        public MarketerRepository(ApplicationDbContext context) : base(context)
        {

        }
        public ApplicationDbContext _DBContext
        {
            get { return Context as ApplicationDbContext; }
        }

        public async Task<Marketer> FindMarketer(string id)
        {
            return await Context.Set<Marketer>().Include(x => x.User).SingleOrDefaultAsync(m => m.MarketerId == id);
        }

        public async Task<IEnumerable<Marketer>> GetAllMarketers()
        {
            return await Context.Set<Marketer>().Include(a => a.User).ToListAsync();
        }

    }
}
