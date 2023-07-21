using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.LeagueDtos;

namespace WebAPIAFA.Repository.IRepository
{
    public interface ILeagueRepository
    {
        public Task CreateLeague(League league);
        public Task<League> GetLeague(int idLeague);
        public Task<bool> LeagueExistCUIT(string cuit);
        public Task<bool> LeagueExistId(int idLeague);
        public Task<bool> LeagueExistName(string name);
        public void UpdateLeague(League league);
        public Task<bool> LeagueExists(int idLeague);
        public Task<bool> LeagueUpdateExistsCUIT(int idLeague, string Cuit);
        public Task<bool> LeagueHaveImage(int idLeague);
        public Task<League> GetLeagueByResponsibleId(int idLoggedUser);
        public Task<League> GetLeague(string leagueName);
        public Task<List<LeagueSelectGetDto>> GetLeaguesSelect();
    }
}
