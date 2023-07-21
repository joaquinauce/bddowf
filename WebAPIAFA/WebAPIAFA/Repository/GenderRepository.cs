using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class GenderRepository: IGenderRepository
    {
        private readonly ApplicationDbContext dbContext;

        public GenderRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Gender> GetGender(int idGender)
        {
            return await dbContext.Genders.FirstOrDefaultAsync(g => g.IdGender == idGender);
        }

        public async Task<IList<Gender>> GetGenders()
        {
            return await dbContext.Genders.OrderBy(g => g.Name).ToListAsync();
        }
    }
}
