using System.Collections;
using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface ICountryRepository
    {
        Task<IList<Country>> GetCountries();
        Task<Country> GetCountry(int idLocation);

    }
}
