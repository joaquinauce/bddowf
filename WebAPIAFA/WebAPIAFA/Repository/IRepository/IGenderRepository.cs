using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IGenderRepository
    {
        public Task<IList<Gender>> GetGenders();
        public Task<Gender> GetGender(int idGender);
    }
}
