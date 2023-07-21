using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.DocumentDtos;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext dbContext;

        public DocumentRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<DocumentGetDto>> GetDocumentsGetDtoByIdFolder(int idFolder)
        {
           return await dbContext.Documents.Where(d => d.IdFolder == idFolder)
                .Select(d => new DocumentGetDto
                {
                    IdDocument = d.IdDocument,
                    Name = d.FileName
                }).ToListAsync();
        }

        public async Task<Document> GetLastDocumentByIdFolder(int idFolder)
        {
            return await dbContext.Documents.OrderBy(x=>x.IdDocument).LastOrDefaultAsync(d => d.IdFolder.Equals(idFolder));
        }

        public async Task<Document> GetFile(int idDocument)
        {
            return await dbContext.Documents.FirstOrDefaultAsync(d => d.IdDocument.Equals(idDocument));
        }

        public async Task CreateDocuments(List<Document> documents)
        {
            await dbContext.Documents.AddRangeAsync(documents);
        }

        public Task<List<Document>> GetDocumentsByIdFolder(int idFolder)
        {
            return dbContext.Documents.Where(d => d.IdFolder.Equals(idFolder)).ToListAsync();
        }

        public Document GetDocumentSync(int idDocument)
        {
            return dbContext.Documents.FirstOrDefault(d => d.IdDocument.Equals(idDocument));
        }

        public List<Document> GetDocumentsByIdFolderSync(int idFolder)
        {
            return dbContext.Documents.Where(d => d.IdFolder.Equals(idFolder)).ToList();
        }

        public void UpdateDocuments(List<Document> documents)
        {
            dbContext.Documents.UpdateRange(documents);
        }

        public async Task<List<Document>> GetDocumentsByIdPass(int idPass)
        {
            return await dbContext.Documents.Where(d => d.IdPass == idPass).ToListAsync();
        }

        public async Task<List<DocumentGetDto>> GetIdDocumentsByIdPass(int idPass)
        {
            return await dbContext.Documents.Where(d => d.IdPass == idPass)
                .Select(d => new DocumentGetDto
                {
                    IdDocument = d.IdDocument,
                    Name = d.FileName
                }).ToListAsync();
        }

        public List<Document> GetDocumentsByIdPassSync(int idPass)
        {
            return dbContext.Documents.Where(d => d.IdPass == idPass).ToList();
        }

        public void UpdateDocument(Document document)
        {
            dbContext.Documents.Update(document);
        }
    }
}
