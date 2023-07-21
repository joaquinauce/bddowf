using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IClubInformationRepository
    {
        public Task<string> GetClubInformation(int IdClubInformation);
        public Task<ClubInformation> GetClubInformationByClub(int IdClub);

        public Task CreateClubInformation(ClubInformation ClubInformation);
        public Task<bool> ClubInformationExists(int IdClubInformation);
        public Task<bool> ClubInformationExistsClub(int IdClub);
        public void DeleteClubInformation(ClubInformation ClubInformation);
        public void UpdateClubInformation(ClubInformation ClubInformation);
    }
}
