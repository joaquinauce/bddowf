using WebAPIAFA.Helpers;
using WebAPIAFA.Models.Dtos.TournamentDivisionDtos;

namespace WebAPIAFA.Services.IServices
{
    public interface IAdministrationService
    {
        Task<ResponseObjectJsonDto> CreateTournamentDivision(TournamentDivisionPostDto tournament);
        Task<ResponseObjectJsonDto> GetCountries();
        Task<ResponseObjectJsonDto> GetDocumentTypes();
        Task<ResponseObjectJsonDto> GetEspeficRoles(string encryptedIdUser);
        Task<ResponseObjectJsonDto> GetGenders();
        Task<ResponseObjectJsonDto> GetLocationsByIdProvince(int idProvince);
        Task<ResponseObjectJsonDto> GetProvinciesByIdCountry(int idCountry);
        Task<ResponseObjectJsonDto> GetReasons();
        Task<ResponseObjectJsonDto> GetRoles();
        Task<ResponseObjectJsonDto> GetUserTypes();
        Task<ResponseObjectJsonDto> GetUserTypesByName(string name);
        Task<ResponseObjectJsonDto> GetTournamentDivision(int idTournament);
    }
}
