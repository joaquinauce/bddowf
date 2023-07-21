using WebAPIAFA.Helpers;
using WebAPIAFA.Models.Dtos.LeagueDtos;
using WebAPIAFA.Models.Dtos.SanctionDtos;

namespace WebAPIAFA.Services.IServices
{
    public interface ILeagueService
    {
        public Task<ResponseObjectJsonDto> CreateLeague(LeaguePostDto leagueDto);
        public Task<ResponseObjectJsonDto> GetLeagueByResponsibleId(string encryptedIdUser);
        public Task<ResponseObjectJsonDto> GetImageLeague(string encryptedIdUser);
        public Task<ResponseObjectJsonDto> GetPlayersByLeague(int idLeague);
        public Task<ResponseObjectJsonDto> GetPlayersByLeagueResponsibleDiscipline(string cuil, string encryptedIdUser);
        public Task<ResponseObjectJsonDto> UpdateLeague(LeaguePatchDto leagueDto);
        public Task<ResponseObjectJsonDto> GetLeagueSelect();
    }
}
