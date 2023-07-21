using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IPassTypeRepository
    {
        public Task<List<PassType>> GetPassType();

    }
}
