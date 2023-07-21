using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class PassStepRepository : IPassStepRepository
    {
        ApplicationDbContext dbContext;
        public PassStepRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreatePassStep(PassStep passStep)
        {
            await dbContext.PassSteps.AddAsync(passStep);
        }

        public async Task<PassStep> GetCurrentPassStep(int idPass)
        {
            return await dbContext.PassSteps.FirstOrDefaultAsync(pS => pS.IdPass == idPass && pS.IsCurrent);
        }

        public PassStep GetCurrentPassStepSync(int idPass)
        {
            return dbContext.PassSteps.FirstOrDefault(pS => pS.IdPass == idPass && pS.IsCurrent);
        }

        public async Task<PassStep> GetPassStep(int idPassStep)
        {
            return await dbContext.PassSteps.FirstOrDefaultAsync(pS => pS.IdPassStep == idPassStep);
        }

        public async Task<List<PassStep>> GetPassStepsByIdPass(int idPass)
        {
            return await dbContext.PassSteps
                .Where(pS => pS.IdPass == idPass)
                .OrderBy(pS => pS.OrderNumber)
                .ToListAsync();
        }

        public PassStep GetPassStepSync(int idPassStep)
        {
            return dbContext.PassSteps.FirstOrDefault(pS => pS.IdPassStep == idPassStep);

        }

        public void UpdateSteps(List<PassStep> passSteps)
        {
            dbContext.PassSteps.UpdateRange(passSteps);
        }
    }
}
