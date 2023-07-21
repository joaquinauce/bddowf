using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.PlayerDtos;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class PlayerRepository: IPlayerRepository
    {
        private readonly ApplicationDbContext dbContext;

        public PlayerRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreatePlayer(Player player)
        {
            await dbContext.Players.AddAsync(player);
        }

        public async Task<Player> GetPlayer(int idUser)
        {
            return await dbContext.Players.FirstOrDefaultAsync(p => p.IdUser == idUser);
        }

        public async Task<IList<Player>> GetPlayers()
        {
            return await dbContext.Players.Include(u => u.User).ThenInclude(ut => ut.UserType).ToListAsync();
        }

        public async Task<IList<Player>> GetPlayersByIdClub(int idClub)
        {
            return await dbContext.Players.Include(u => u.User).ThenInclude(ut=>ut.UserType).Where(u => u.User.IdClub.Equals(idClub) && u.User.UserType.Name.Equals("Jugador")).ToListAsync();
        }

        public async Task<IList<PlayerLeagueGetDto>> GetPlayersByLeague(int idLeague)
        {
            return await (from leagues in dbContext.Leagues
                          join clubs in dbContext.Clubs on leagues.IdLeague equals clubs.IdLeague
                          join users in dbContext.Users on clubs.IdClub equals users.IdClub
                          join userTypes in dbContext.UserTypes on users.IdUserType equals userTypes.IdUserType
                          where leagues.IdLeague.Equals(idLeague) && userTypes.Name.ToLower().Equals("jugador")
                          select new PlayerLeagueGetDto
                          {
                              Name = users.Name,
                              LastName = users.LastName,
                              BussinessNameClub = clubs.BussinessName,
                              LogoClub = clubs.LogoClub
                          }).ToListAsync();
        }

        public void UpdatePlayer(Player player)
        {
            dbContext.Players.Update(player);
        }
    }
}
