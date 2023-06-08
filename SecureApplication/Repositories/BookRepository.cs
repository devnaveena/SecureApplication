using Contracts;
using Contracts.IRepositories;
using Entities;
using Entities.Models;

namespace Repositories
{
    public class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        public BookRepository(RepositoryContext repositoryContext)
            :base(repositoryContext)
        {
        }
    }
}