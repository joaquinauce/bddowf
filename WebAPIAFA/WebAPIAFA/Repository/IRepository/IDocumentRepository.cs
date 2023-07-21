using WebAPIAFA.Models.Dtos.DocumentDtos;
using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IDocumentRepository
    {
        public Task<List<DocumentGetDto>> GetDocumentsGetDtoByIdFolder(int idFolder);
        public Task<Document> GetLastDocumentByIdFolder(int idFolder);
        public Task<List<Document>> GetDocumentsByIdFolder(int idFolder);
        public Task<Document> GetFile(int idDocument);
        public Document GetDocumentSync(int idDocument);
        public List<Document> GetDocumentsByIdFolderSync(int idFolder);
        public void UpdateDocuments(List<Document> documents);
        Task CreateDocuments(List<Document> documents);
        public Task<List<Document>> GetDocumentsByIdPass(int idPass);
        public Task<List<DocumentGetDto>> GetIdDocumentsByIdPass(int idPass);
        public List<Document> GetDocumentsByIdPassSync(int idPass);
        public void UpdateDocument(Document document);
    }
}
