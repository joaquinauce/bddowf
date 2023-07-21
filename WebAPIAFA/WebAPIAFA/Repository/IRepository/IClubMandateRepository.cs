using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IClubMandateRepository
    {
        public Task CreateMandate(ClubMandate mandate);
        public Task<bool> ClubMandateExistId(int idClubMandate);
        public Task<ClubMandate> GetCurrentClubMandate(int idClubMandate);
    }
}
