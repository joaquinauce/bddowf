using Microsoft.EntityFrameworkCore;
using System.Collections;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext dbContext;
        public CountryRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IList<Country>> GetCountries()
        {
            return await dbContext.Countries.ToListAsync();
        }

        public async Task<Country> GetCountry(int idLocation)
        {

            return await (from locations in dbContext.Locations
                          join provinces in dbContext.Provinces on locations.IdProvince equals provinces.IdProvince
                          join countries in dbContext.Countries on provinces.IdCountry equals countries.IdCountry
                          where locations.IdLocation == idLocation
                          select countries).FirstOrDefaultAsync();
        }
    }
}
