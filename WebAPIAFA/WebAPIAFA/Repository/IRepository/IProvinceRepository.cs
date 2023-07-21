using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IProvinceRepository
    {
        Task<IList<Province>> GetProvincesByIdCountry(int idCountry);
    }
}
