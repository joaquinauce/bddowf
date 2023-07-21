using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IReasonRepository
    {
        Task<Reason> GetReason(int idReason);
        Task<IList<Reason>> GetReasons();
    }
}
