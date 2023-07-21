using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IDocumentTypeRepository
    {
        public Task<List<DocumentType>> GetDocumentTypes();
        public Task<DocumentType> GetDocumentType(int IdDocumentType);
    }
}
