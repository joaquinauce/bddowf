using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class LocationRepository: ILocationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LocationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IList<Location>> GetLocationsByIdProvince(int idProvince)
        {
            return await dbContext.Locations.Where(lo => lo.IdProvince == idProvince).ToListAsync();
        }

    }
}
