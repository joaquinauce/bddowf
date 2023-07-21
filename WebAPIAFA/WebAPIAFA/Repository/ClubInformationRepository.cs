using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class ClubInformationRepository : IClubInformationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ClubInformationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> ClubInformationExists(int IdClubInformation)
        {
            return await dbContext.ClubInformation.AnyAsync(cI => cI.IdClubInformation.Equals(IdClubInformation));
        }

        public async Task<bool> ClubInformationExistsClub(int IdClub)
        {
            return await dbContext.ClubInformation.AnyAsync(cI => cI.IdClub == IdClub);
        }

        public async Task CreateClubInformation(ClubInformation ClubInformation)
        {
            await dbContext.ClubInformation.AddAsync(ClubInformation);
        }

        public void DeleteClubInformation(ClubInformation ClubInformation)
        {
            dbContext.ClubInformation.Remove(ClubInformation);
        }

        public async Task<ClubInformation> GetClubInformationByClub(int IdClub)
        {
            return await dbContext.ClubInformation.FirstOrDefaultAsync(cI => cI.IdClub == IdClub);
        }

        public async Task<string> GetClubInformation(int IdClubInformation)
        {
            ClubInformation clubInfo = await dbContext.ClubInformation.AsNoTracking().FirstOrDefaultAsync(cI => cI.IdClubInformation.Equals(IdClubInformation));
            return clubInfo.StadiumImage;
        }

        public void UpdateClubInformation(ClubInformation ClubInformation)
        {
            dbContext.ClubInformation.Update(ClubInformation);
        }
    }
}
