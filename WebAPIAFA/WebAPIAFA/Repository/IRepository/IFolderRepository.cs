using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.FolderDtos;
using WebAPIAFA.Models.Dtos.PlayerDtos;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IFolderRepository
    {
        public Task<FolderGetDto> GetFolderGetDto(int idFolder);
        public Task<Folder> GetFolder(int idFolder);
        public Task<List<FolderTrayGetDto>> GetFolderConsult(int idUser, FolderFilterDto filters);
        public Task<List<FolderTrayGetDto>> GetFolderReceived(int idUser, FolderFilterDto filters);
        public Task<Folder> GetFolderByIdUser(int idUser);
        public Task<IList<Folder>> GetFoldersByIdAffectedUser(int idUser);
        public Task<IList<Folder>> GetFoldersByIdAffectedUserAndFilters(int idUser, FolderFilterDto filters);
        public Task<Folder> GetLastFolderByIdUser(int idUser);
        public Task CreateFolder(Folder folder);
        public Task<IList<PlayerContractHistoryGetDto>> GetPlayerContractHistory(int idUser);
    }
}
