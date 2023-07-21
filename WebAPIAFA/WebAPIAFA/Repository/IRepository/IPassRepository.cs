using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.PassDtos;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IPassRepository
    {
        public Task<Pass> GetPass(int idPass);
        public Task<Pass> GetPassDetail(int idPass);
        public Task<bool> PassExists(int idPass);
        public void UpdatePass(Pass pass);
        public Task<IList<Pass>> GetPassesByAffectedUser(int IdAffectedUser);
        public Task<IList<Pass>> GetPassesByAffectedUserAndFilters(int IdAffectedUser, PassPlayerFilterDto filters);
        public Task<IList<PassGetDto>> GetPassReceived(int idUser, PassFilterDto filters);
        public Task<IList<PassGetDto>> GetPassLeagueReceived(int idLeague, PassFilterDto filters);
        public Task<IList<PassGetDto>> GetPassConsultPlayer(int idUser, PassFilterDto filters);
        public Task<IList<PassGetDto>> GetPassConsultLeague(int idLeague, PassFilterDto filters);
        public Task<IList<PassGetDto>> GetPassConsult(PassFilterDto filters);
        public Task CreatePass(Pass pass);
    }
}
