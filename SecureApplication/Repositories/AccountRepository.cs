using Contracts;
using Contracts.IRepositories;
using Entities;
using Entities.Models;

namespace Repositories
{
    public class AccountRepository : RepositoryBase<User>, IAccountRepository
    {
        protected readonly RepositoryContext _context;
        public AccountRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
            _context = repositoryContext;

        }
       
        public bool UserExists(string email, long phone)
        {
            return _context.User.Any(a => (a.Email == email || a.Phone == phone) && a.IsActive == true);
        }
    }
}