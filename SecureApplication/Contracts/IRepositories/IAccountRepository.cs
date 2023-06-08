using Contracts.IRepositories;
using Entities.Models;

namespace Contracts.IRepositories
{
    public interface IAccountRepository : IRepositoryBase<User>
    {
        /// <summary>
        /// Checks if an user account already exists in the DB
        /// </summary>
        /// <returns>boolen</returns>
        public bool UserExists(string email, long phone);
    }
}