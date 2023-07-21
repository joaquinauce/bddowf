using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.ClubDtos;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class ClubRepository : IClubRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ClubRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<bool> ClubExistId(int idClub)
        {
            return await dbContext.Clubs.AnyAsync(c => c.IdClub == idClub );
        }
        public async Task<bool> ClubExistBussinessName(string bussinessName)
        {
            return await dbContext.Clubs.AnyAsync(c => c.BussinessName.ToUpper().Equals(bussinessName.ToUpper()));
        }

        public async Task<bool> ClubExistCuit(string cuit)
        {
            return await dbContext.Clubs.AnyAsync(c => c.CUIT.Equals(cuit));
        }

        public async Task<bool> ClubExistEmail(string email)
        {
            return await dbContext.Clubs.AnyAsync(c => c.Email.Equals(email));
        }

        public async Task CreateClub(Club club)
        {
            await dbContext.Clubs.AddAsync(club);
        }

        public async Task<Club> GetClubInfoById(int idClub)
        {
            return await dbContext.Clubs.FirstOrDefaultAsync(c => c.IdClub.Equals(idClub));
        }

        public async Task<IList<Club>> GetClubsByTournamentDivision(int idDivisionTournament)
        {
            return await (from clubs in dbContext.Clubs
                         join teams in dbContext.Teams
                         on clubs.IdClub equals teams.IdClub
                         where teams.IdTournamentDivision.Equals(idDivisionTournament)
                         select clubs).Distinct().ToListAsync();
        }

        public async Task<ICollection<ClubSponsor>> GetClubSponsorsByClub(int idClub)
        {
            return await dbContext.ClubSponsors.Include("sponsor").Where(c => c.IdClub.Equals(idClub)).ToListAsync();
        }

        public async Task<ICollection<Club>> GetClubsByLeague(int idLeague)
        {
            return await dbContext.Clubs.Include(l => l.League).Where(c => c.IdLeague.Equals(idLeague)).ToListAsync();
        }

        public async Task<ICollection<Sponsor>> GetNonclubSponsors(int idClub)
        {
            return await (from sponsors in dbContext.Sponsors
                            join clubSponsors in dbContext.ClubSponsors
                            on sponsors.IdSponsor equals clubSponsors.IdSponsor into fj
                            from clubSponsors in fj.DefaultIfEmpty()
                            where clubSponsors.IdClub != idClub || clubSponsors.IdClub.Equals(null)
                            select sponsors).ToListAsync();
        }

        public async Task CreateClubSponsor(List<ClubSponsor> clubSponsor)
        {
            await dbContext.ClubSponsors.AddRangeAsync(clubSponsor);
        }

        public async Task<ICollection<ClubSponsor>> GetListClubSponsor(int idClub)
        {
            return await dbContext.ClubSponsors.Where(cS => cS.IdClub == idClub).ToListAsync();
        }

        public void DeleteClubSponsor(ClubSponsor clubSponsor)
        {
            dbContext.ClubSponsors.Remove(clubSponsor);
        }

        public async Task<List<ClubGetDto>> GetClubs()
        {
            return await dbContext.Clubs.Select(c => new ClubGetDto
            {
                IdClub = c.IdClub,
                BussinessName = c.BussinessName,
            }).ToListAsync();
        }

        public void UpdateClub(Club club)
        {
            dbContext.Update(club);
        }

        public async Task<bool> ClubUpdateExistsCUIT(int idClub, string Cuit)
        {
            return await dbContext.Clubs.AnyAsync(u => u.CUIT == Cuit && u.IdClub != idClub);
        }

        public async Task<bool> ClubUpdateExistsMail(int idClub, string email)
        {
            return await dbContext.Clubs.AnyAsync(u => u.Email == email && u.IdClub != idClub);
        }

        public async Task<Club> GetClub(int idClub)
        {
            return await dbContext.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.IdClub == idClub);
        }

        public async Task<Club> GetClubByResponsibleId(int idLoggedUser)
        {
            ResponsibleXClub respXClub = await dbContext.ResponsiblesXClub.
                FirstOrDefaultAsync(rXC => rXC.IdResponsible == idLoggedUser);

            if (respXClub == null)
            {
                return null;
            }

            return await dbContext.Clubs.FirstOrDefaultAsync( c => c.IdClub == respXClub.IdClub );
        }

        public async Task<List<ClubGetDto>> GetLeagueClubs(int idLeague)
        {
            return await dbContext.Clubs
                .Where(c => c.IdLeague == idLeague)
                .Select(c => new ClubGetDto
                {
                    IdClub = c.IdClub,
                    BussinessName = c.BussinessName
                })
                .ToListAsync();
        }
    }
}
