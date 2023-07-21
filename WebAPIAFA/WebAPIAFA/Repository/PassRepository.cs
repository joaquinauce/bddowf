using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.PassDtos;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class PassRepository : IPassRepository
    {
        ApplicationDbContext dbContext;
        public PassRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreatePass(Pass pass)
        {
            await dbContext.Passes.AddAsync(pass);
        }

        public async Task<Pass> GetPass(int idPass)
        {
            return await dbContext.Passes.FirstOrDefaultAsync(p => p.IdPass == idPass);
        }
        public async Task<Pass> GetPassDetail(int idPass)
        {
            return await dbContext.Passes.Include(cf => cf.ClubFrom).ThenInclude(lf => lf.League).Include(ct => ct.ClubTo).ThenInclude(lt => lt.League).Include(us => us.User).Include(pt => pt.PassType).FirstOrDefaultAsync(p => p.IdPass == idPass);
        }

        public async Task<bool> PassExists(int idPass)
        {
            return await dbContext.Passes.AnyAsync(p => p.IdPass == idPass);
        }

        public void UpdatePass(Pass pass)
        {
            dbContext.Passes.Update(pass);
        }

        public async Task<IList<Pass>> GetPassesByAffectedUser(int IdAffectedUser)
        {
            return await dbContext.Passes
                .Include(cf => cf.ClubFrom)
                .Include(ct => ct.ClubTo)
                .Include(p => p.PassType)
                .Where(p => p.IdAffectedUser == IdAffectedUser).ToListAsync();
        }

        public async Task<IList<Pass>> GetPassesByAffectedUserAndFilters(int IdAffectedUser, PassPlayerFilterDto filters)
        {
            IList<Pass> passes = await dbContext.Passes.Include(cf => cf.ClubFrom).Include(ct => ct.ClubTo)
                .Include(p => p.PassType).Where(p => p.IdAffectedUser == IdAffectedUser).ToListAsync();

            if (filters.IdPassType != null && filters.IdPassType != 0)
            {
                passes = passes.Where(p => p.IdPassType.Equals(filters.IdPassType)).ToList();
            }

            if (filters.IsCurrent != null)
            {
                if (filters.IsCurrent == true)
                {
                    passes = passes.Where(p => p.EndDate == null || ((DateTime)p.EndDate).Date >= DateTime.Now).ToList();
                }
                else
                {
                    passes = passes.Where(p => p.EndDate != null && ((DateTime)p.EndDate).Date <= DateTime.Now).ToList();
                }
            }

            return passes;
        }

        public async Task<IList<PassGetDto>> GetPassReceived(int idUser, PassFilterDto filters)
        {
            IList<PassGetDto> listPassGeneral = await dbContext.PassSteps
                .Include(s => s.Pass.ClubTo)
                .Include(s => s.Pass.User)
                .Where(s => s.IdUser == idUser && s.IsCurrent && !s.IsDone)
                .Select(s => new PassGetDto
                {
                   IdPass = s.IdPass,
                   Name = "Pase " + s.Pass.User.LastName + " " + s.Pass.User.Name,
                   LogoClub = s.Pass.ClubTo.LogoClub,
                   BeginDate = s.Pass.BeginDate,
                   Accepted = (bool)s.Pass.Accepted,
                   IdPassType = s.Pass.IdPassType,
                   EndDate = s.Pass.EndDate
                }).ToListAsync();

            if (filters.Name != "" && filters.Name != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.Name.ToLower().Contains(filters.Name.ToLower())).ToList();
            }

            if (filters.IdPassType != null && filters.IdPassType != 0)
            {
                listPassGeneral = listPassGeneral.Where(f => f.IdPassType == filters.IdPassType).ToList();
            }

            if (filters.BeginDate != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.BeginDate == null || ((DateTime)f.BeginDate).Date >= filters.BeginDate).ToList();
            }

            if (filters.EndDate != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.EndDate == null || ((DateTime)f.EndDate).Date <= filters.EndDate).ToList();
            }

            return listPassGeneral;
        }

        public async Task<IList<PassGetDto>> GetPassLeagueReceived(int idLeague, PassFilterDto filters)
        {
            ActionType actionTypeValidate = await dbContext.ActionTypes
                .FirstOrDefaultAsync(aT => aT.Name.ToUpper() == "VALIDACION");

            IList<PassGetDto> listPassGeneral = await dbContext.PassSteps
                .Include(s => s.Pass).ThenInclude(p => p.ClubTo)
                .Include(s => s.Pass).ThenInclude(p => p.User)
                .Where(s => s.IdLeague == idLeague 
                && s.IdActionType == actionTypeValidate.IdActionType 
                && s.IsCurrent && !s.IsDone)
                .Select(s => new PassGetDto
                {
                   IdPass = s.IdPass,
                   Name = "Pase " + s.Pass.User.LastName + " " + s.Pass.User.Name,
                   LogoClub = s.Pass.ClubTo.LogoClub,
                   BeginDate = s.Pass.BeginDate,
                   Accepted = (bool)s.Pass.Accepted,
                   IdPassType = s.Pass.IdPassType,
                   EndDate = s.Pass.EndDate
                }).ToListAsync();

            if (filters.Name != "" && filters.Name != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.Name.ToLower().Contains(filters.Name.ToLower())).ToList();
            }

            if (filters.IdPassType != null && filters.IdPassType != 0)
            {
                listPassGeneral = listPassGeneral.Where(f => f.IdPassType == filters.IdPassType).ToList();
            }

            if (filters.BeginDate != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.BeginDate == null || ((DateTime)f.BeginDate).Date >= filters.BeginDate).ToList();
            }

            if (filters.EndDate != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.EndDate == null || ((DateTime)f.EndDate).Date <= filters.EndDate).ToList();
            }

            return listPassGeneral;
        }

        public async Task<IList<PassGetDto>> GetPassConsultPlayer(int idUser, PassFilterDto filters)
        {
            IList<PassGetDto> listPassGeneral = await dbContext.PassSteps
                .Include(s => s.Pass.ClubTo)
                .Include(s => s.Pass.User)
                .Include(s => s.User)
                .Where(s => s.IdUser == idUser && s.IsDone)
               .Select(s => new PassGetDto
               {
                   IdPass = s.IdPass,
                   Name = "Pase " + s.Pass.User.LastName + " " + s.Pass.User.Name,
                   LogoClub = s.Pass.ClubTo.LogoClub,
                   BeginDate = s.Pass.BeginDate,
                   Accepted = (bool)s.Pass.Accepted,
                   IdPassType = s.Pass.IdPassType,
                   EndDate = s.Pass.EndDate
               }).ToListAsync();

            if (filters.Name != "" && filters.Name != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.Name.ToLower().Contains(filters.Name.ToLower())).ToList();
            }

            if (filters.IdPassType != null && filters.IdPassType != 0)
            {
                listPassGeneral = listPassGeneral.Where(f => f.IdPassType == filters.IdPassType).ToList();
            }

            if (filters.BeginDate != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.BeginDate == null || ((DateTime)f.BeginDate).Date >= filters.BeginDate).ToList();
            }

            if (filters.EndDate != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.EndDate == null || ((DateTime)f.EndDate).Date <= filters.EndDate).ToList();
            }

            return listPassGeneral;
        }

        public async Task<IList<PassGetDto>> GetPassConsultLeague(int idLeague, PassFilterDto filters)
        {
            IList<PassGetDto> listPassGeneral = await dbContext.PassSteps
                .Include(s => s.Pass.ClubTo)
                .Include(s => s.Pass.User)
                .Include(p => p.User)
                .Where(s => s.IdLeague == idLeague && s.IsDone)
               .Select(s => new PassGetDto
               {
                   IdPass = s.IdPass,
                   Name = "Pase " + s.Pass.User.LastName + " " + s.Pass.User.Name,
                   LogoClub = s.Pass.ClubTo.LogoClub,
                   BeginDate = s.Pass.BeginDate,
                   Accepted = (bool)s.Pass.Accepted,
                   IdPassType = s.Pass.IdPassType,
                   EndDate = s.Pass.EndDate
               }).Distinct().ToListAsync();

            if (filters.Name != "" && filters.Name != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.Name.ToLower().Contains(filters.Name.ToLower())).ToList();
            }

            if (filters.IdPassType != null && filters.IdPassType != 0)
            {
                listPassGeneral = listPassGeneral.Where(f => f.IdPassType == filters.IdPassType).ToList();
            }

            if (filters.BeginDate != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.BeginDate == null || ((DateTime)f.BeginDate).Date >= filters.BeginDate).ToList();
            }

            if (filters.EndDate != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.EndDate == null || ((DateTime)f.EndDate).Date <= filters.EndDate).ToList();
            }

            return listPassGeneral;
        }

        public async Task<IList<PassGetDto>> GetPassConsult(PassFilterDto filters)
        {
            IList<PassGetDto> listPassGeneral = await dbContext.PassSteps
                .Include(s => s.Pass.ClubTo)
                .Include(s => s.Pass.User)
                .Include(s => s.User)
                .Where(s => s.IsDone)
               .Select(s => new PassGetDto
               {
                   IdPass = s.IdPass,
                   Name = "Pase " + s.Pass.User.LastName + " " + s.Pass.User.Name,
                   LogoClub = s.Pass.ClubTo.LogoClub,
                   BeginDate = s.Pass.BeginDate,
                   Accepted = (bool)s.Pass.Accepted,
                   IdPassType = s.Pass.IdPassType,
                   EndDate = s.Pass.EndDate,
                   IdLeague = s.Pass.ClubTo.IdLeague,
                   IdClub = s.Pass.ClubTo.IdClub
               }).Distinct().ToListAsync();

            if (filters.Name != "" && filters.Name != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.Name.ToLower().Contains(filters.Name.ToLower())).ToList();
            }

            if (filters.IdPassType != null && filters.IdPassType != 0)
            {
                listPassGeneral = listPassGeneral.Where(f => f.IdPassType == filters.IdPassType).ToList();
            }

            if (filters.BeginDate != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.BeginDate == null || ((DateTime)f.BeginDate).Date >= filters.BeginDate).ToList();
            }

            if (filters.EndDate != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.EndDate == null || ((DateTime)f.EndDate).Date <= filters.EndDate).ToList();
            }

            if (filters.IdLeague != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.IdLeague == filters.IdLeague).ToList();
            }

            if (filters.IdClub != null)
            {
                listPassGeneral = listPassGeneral.Where(f => f.IdClub == filters.IdClub).ToList();
            }

            return listPassGeneral;
        }
    }
}
