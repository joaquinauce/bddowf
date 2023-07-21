using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.PlayerDtos;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IPlayerRepository
    {
        public Task CreatePlayer(Player player);
        public Task<IList<Player>> GetPlayers();
        public Task<IList<Player>> GetPlayersByIdClub(int idClub);
        public Task<IList<PlayerLeagueGetDto>> GetPlayersByLeague(int idLeague);
        public Task<Player> GetPlayer(int idUser);
        void UpdatePlayer(Player player);
    }
}
