using System.Threading.Tasks;

namespace File.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public Task Commit();
        public Task Rollback();
    }
}
