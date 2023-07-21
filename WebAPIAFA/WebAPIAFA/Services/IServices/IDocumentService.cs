using WebAPIAFA.Helpers;
using WebAPIAFA.Models;

namespace WebAPIAFA.Services.IServices
{
    public interface IDocumentService
    {
        Task<Document> GetFile(int idDocument);
        Task<ResponseObjectJsonDto> ValidateSignature(int idDocument);
        Task<ResponseObjectJsonDto> GetAffectedUserDocuments(int idAffectedUSer);
    }
}
