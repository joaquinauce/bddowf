using Microsoft.EntityFrameworkCore.Storage;

namespace WebAPIAFA.Entity
{
    public interface IDbOperation
    {
        public Task<bool> Save();
        public bool SaveSync();
        public Task<IDbContextTransaction> BeginTransaction();
        public Task Rollback(IDbContextTransaction dbContextTransaction);
        public Task Commit(IDbContextTransaction dbContextTransaction);
    }
}
