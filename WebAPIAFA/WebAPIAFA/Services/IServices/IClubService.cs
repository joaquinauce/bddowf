using WebAPIAFA.Helpers;
using WebAPIAFA.Models.Dtos.ClubDtos;
using WebAPIAFA.Models.Dtos.ClubInformationDtos;
using WebAPIAFA.Models.Dtos.ClubSponsorsDto;

namespace WebAPIAFA.Services.IServices
{
    public interface IClubService
    {
        Task<ResponseObjectJsonDto> CreateClub(ClubCreateDto clubDto);
        Task<ResponseObjectJsonDto> GetClubStaffs();
        Task<ResponseObjectJsonDto> GetClubInfoById(int idClub);
        Task<ResponseObjectJsonDto> GetClubStadiumInfoById(int idClub);
        Task<ResponseObjectJsonDto> UpdateClubInformation(ClubInformationPatchDto clubInfoDto);
        Task<ResponseObjectJsonDto> GetTournamentDivisions();
        Task<ResponseObjectJsonDto> GetClubsByTournamentDivision(int idTournamentDivision);
        Task<ResponseObjectJsonDto> GetClubsByLeague(int idLeague);
        Task<FileStream> GetClubStatuteById(int idClub);
        Task<ResponseObjectJsonDto> GetSponsorsByIdClub(int idClub);
        Task<ResponseObjectJsonDto> GetNonclubSponsors(int idClub);
        Task<ResponseObjectJsonDto> CreateClubSponsor(int idClub, List<ClubSponsorPostDto> lstClubSponsorDto);
        Task<ResponseObjectJsonDto> GetClubs();
        Task<ResponseObjectJsonDto> GetClubStaffsByIdClub(int idClub);
        Task<ResponseObjectJsonDto> GetImageClub(string encryptedIdUser);
        Task<ResponseObjectJsonDto> CreateClubInformation(ClubInformationCreateDto clubInfoDto);
        Task<ResponseObjectJsonDto> UpdateFirstClub(ClubPatchDto clubDto, string idUserLogged);
        Task<ResponseObjectJsonDto> GetClubByResponsibleId(string encryptedIdUser);
        Task<ResponseObjectJsonDto> GetLeagueClubs(string encryptedIdUser);
    }
}
