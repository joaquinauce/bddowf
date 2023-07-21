using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface ILocationRepository
    {
        Task<IList<Location>> GetLocationsByIdProvince(int idProvince);
    }
}
