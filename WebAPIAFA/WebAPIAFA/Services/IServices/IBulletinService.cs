using WebAPIAFA.Helpers;
using WebAPIAFA.Models.Dtos.BulletinDtos;
using WebAPIAFA.Models.Dtos.PassDtos;

namespace WebAPIAFA.Services.IServices
{
    public interface IBulletinService
    {
        Task<ResponseObjectJsonDto> GetBulletinTypes();
        Task<ResponseObjectJsonDto> CreateBulletin(BulletinCreateDto bulletinDto);
        Task<ResponseObjectJsonDto> GetBulletins(BulletinFiltersDto bulletinFilters);
        Task<ResponseObjectJsonDto> GetStatements(BulletinFiltersDto bulletinFilters, string encryptedIdUser);
        Task<ResponseObjectJsonDto> GetStatementsReceived(BulletinFiltersDto bulletinFilters, string encryptedIdUser);
        Task<ResponseObjectJsonDto> GetStatementsConsult(BulletinFiltersDto bulletinFilters, string encryptedIdUser);
        Task<ResponseObjectJsonDto> PerformAction(string encryptedUserName, StatementSignDto statementSignDto);
        ResponseObjectJsonDto SignEncustodyLote(string token, string cuil, string otherParams);
        Task<ResponseObjectJsonDto> AddStatementSubscribers(StatementSubscribersAddDto subscribersAddDto);
        Task<ResponseObjectJsonDto> GetStatementCounters(string encryptedIdUser);
    }
}
