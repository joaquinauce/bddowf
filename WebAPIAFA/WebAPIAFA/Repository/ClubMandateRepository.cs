using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class ClubMandateRepository : IClubMandateRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ClubMandateRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> ClubMandateExistId(int idClubMandate)
        {
            return await dbContext.ClubMandates.AnyAsync(c => c.IdClubMandate == idClubMandate);
        }

        public async Task CreateMandate(ClubMandate mandate)
        {
            await dbContext.ClubMandates.AddAsync(mandate);
        }

        public async Task<ClubMandate> GetCurrentClubMandate(int idClubMandate)
        {
            return await dbContext.ClubMandates.
                FirstOrDefaultAsync(cM => cM.IdClubMandate == idClubMandate 
                && cM.MandateExpiration == null);
        }
    }
}
