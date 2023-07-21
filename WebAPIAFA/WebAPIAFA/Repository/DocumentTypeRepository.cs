using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        ApplicationDbContext dbContext;
        public DocumentTypeRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<DocumentType> GetDocumentType(int IdDocumentType)
        {
            return await dbContext.DocumentTypes.FirstOrDefaultAsync(dT => dT.IdDocumentType == IdDocumentType);
        }

        public async Task<List<DocumentType>> GetDocumentTypes()
        {
            return await dbContext.DocumentTypes.ToListAsync();
        }
    }
}
