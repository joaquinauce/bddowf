using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class PassTypeRepository: IPassTypeRepository
    {
        ApplicationDbContext dbContext;
        public PassTypeRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<PassType>> GetPassType()
        {
            return await dbContext.PassTypes.ToListAsync();
        }
    }
}
