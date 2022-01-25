using System.Threading.Tasks;
using File.Domain.Repositories;
using File.Infrastructure.Persistence.Contexts;

namespace File.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FileContext _databaseContext;

        public UnitOfWork(FileContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task Commit()
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task Rollback()
        {
            await _databaseContext.DisposeAsync();
        }
    }
}
