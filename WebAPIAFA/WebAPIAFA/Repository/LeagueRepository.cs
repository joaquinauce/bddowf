using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.LeagueDtos;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class LeagueRepository: ILeagueRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LeagueRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateLeague(League league)
        {
            await dbContext.Leagues.AddAsync(league);
        }
        public async Task<League> GetLeague(int idLeague)
        {
            return await dbContext.Leagues.AsNoTracking().FirstOrDefaultAsync(u => u.IdLeague == idLeague);
        }
        public async Task<bool> LeagueExists(int idLeague)
        {
            return await dbContext.Leagues.AnyAsync(u => u.IdLeague == idLeague);
        }
        public async Task<bool> LeagueUpdateExistsCUIT(int idLeague, string Cuit)
        {
            return await dbContext.Leagues.AnyAsync(u => u.CUIT == Cuit && u.IdLeague != idLeague);
        }

        public async Task<bool> LeagueHaveImage(int idLeague)
        {
            return await dbContext.Leagues.AnyAsync(u => u.IdLeague == idLeague && u.Image != null);
        }

        public async Task<bool> LeagueExistCUIT(string cuit)
        {
            return await dbContext.Leagues.AnyAsync(c => c.CUIT == cuit);
        }

        public async Task<bool> LeagueExistId(int idLeague)
        {
            return await dbContext.Leagues.AnyAsync(c => c.IdLeague == idLeague);
        }

        public async Task<bool> LeagueExistName(string name)
        {
            return await dbContext.Leagues.AnyAsync(c => c.Name == name);
        }

        public void UpdateLeague(League league)
        {
            dbContext.Update(league);
        }

        public async Task<League> GetLeagueByResponsibleId(int idLoggedUser)
        {
            ResponsibleXLeague respXLeague = await dbContext.ResponsiblesXLeague
                .FirstOrDefaultAsync(rXL => rXL.IdResponsible == idLoggedUser);

            if (respXLeague == null)
            {
                return null;
            }

            return await dbContext.Leagues.FirstOrDefaultAsync(l => l.IdLeague == respXLeague.IdLeague);
        }

        public async Task<League> GetLeague(string leagueName)
        {
            return await dbContext.Leagues.FirstOrDefaultAsync(l => l.Name == leagueName);
        }

        public async Task<List<LeagueSelectGetDto>> GetLeaguesSelect()
        {
            return await dbContext.Leagues
                 .OrderBy(l => l.Name)
                 .Where(l => l.CUIT.ToUpper() != "NO ASIGNADO")
                 .Select(l => new LeagueSelectGetDto
                 {
                     IdLeague = l.IdLeague,
                     Name = l.Name
                 })
                 .ToListAsync();
        }
    }
}
