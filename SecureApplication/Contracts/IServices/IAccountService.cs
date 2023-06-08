using Entities.Dtos.RequestDto;
using Entities.Dtos.ResponseDto;

namespace Contracts.IServices
{
    public interface IAccountService
    {
        /// <summary>
        /// The function creates a user account in a service and checks if the user already exists in the
        /// database.
        /// </summary>
        /// <param name="user"> </param>
        public void CreateUser(UserDto user);

        /// <summary>
        /// Authenticates the user email and password
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns the Generated Token and TokenType as TokenDto </returns>
        public TokenDto Login(LoginDto login);
    }
}