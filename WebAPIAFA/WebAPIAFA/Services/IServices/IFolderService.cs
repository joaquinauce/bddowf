using WebAPIAFA.Helpers;
using WebAPIAFA.Models.Dtos.FolderDtos;

namespace WebAPIAFA.Services.IServices
{
    public interface IFolderService
    {
        public Task<ResponseObjectJsonDto> CreateFolder(FolderCreateDto folderDto, string mailUser);
        public Task<ResponseObjectJsonDto> GetFolder(int idFolder);
        public Task<ResponseObjectJsonDto> GetFolderConsult(string userName, FolderFilterDto filters);
        public Task<ResponseObjectJsonDto> GetFolderReceived(string userName, FolderFilterDto filters);
        public Task<ResponseObjectJsonDto> GetPlayerContractHistory(string encryptedIdUser);
        public Task<ResponseObjectJsonDto> PerformAction(int idFolder, string userName, List<FolderSignDto> lstFolderSignDto);
        public ResponseObjectJsonDto SignEncustodyLote(string token, string cuil, string otherParams, int idFolder);
        public Task<ResponseObjectJsonDto> GetFoldersByUser(int idUser, FolderFilterDto filters);

    }
}
