using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.ClubDtos;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IClubRepository
    {
        public Task<bool> ClubExistId(int idClub);
        public Task<bool> ClubExistBussinessName(string bussinessName);
        public Task<bool> ClubExistCuit(string cuit);
        public Task<bool> ClubExistEmail(string email);
        public Task CreateClub(Club club);
        public Task<Club> GetClubInfoById(int idClub);
        public Task<ICollection<Club>> GetClubsByLeague(int idLeague);
        public Task<IList<Club>> GetClubsByTournamentDivision(int idDivisionTournament);
        public Task<ICollection<ClubSponsor>> GetClubSponsorsByClub(int idClub);
        public Task CreateClubSponsor(List<ClubSponsor> clubSponsor);
        public Task<ICollection<ClubSponsor>> GetListClubSponsor(int idClub);
        public Task<ICollection<Sponsor>> GetNonclubSponsors(int idClub);
        public void DeleteClubSponsor(ClubSponsor clubSponsor);
        public Task<List<ClubGetDto>> GetClubs();

        public void UpdateClub(Club club);

        public Task<bool> ClubUpdateExistsCUIT(int idClub, string Cuit);
        public Task<bool> ClubUpdateExistsMail(int idClub, string email);
        public Task<Club> GetClub(int idClub);
        public Task<Club> GetClubByResponsibleId(int idLoggedUser);
        public Task<List<ClubGetDto>> GetLeagueClubs(int idLeague);
    }
}
