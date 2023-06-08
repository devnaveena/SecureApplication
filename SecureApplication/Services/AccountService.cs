using Contracts.IServices;
using Entities.Dtos.RequestDto;
using Repositories;
using AutoMapper;
using Contracts.IRepositories;
using Entities.Models;
using Exceptions;
using System.Security.Cryptography;
using System.Text;
using Entities.Dtos.ResponseDto;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Services
{
    public class AccountService : IAccountService
    {

        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountService> _logger;
        public AccountService(IMapper mapper, IRepositoryWrapper repositoryWrapper, IAccountRepository accountRepository,
                                  IConfiguration configuration, ILogger<AccountService> logger)
        {
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _accountRepository = accountRepository;
            _repositoryWrapper = repositoryWrapper;
        }

        /// <summary>
        /// The function creates a user account in a service and checks if the user already exists in the
        /// database.
        /// </summary>
        /// <param name="UserDto">UserDto is a data transfer object that contains information about a
        /// user, such as their email, phone number, and other relevant details. It is used as a
        /// parameter in the CreateUser method to create a new user account.</param>
        public void CreateUser(UserDto user)
        {
            _logger.LogInformation("Creating a new user with email: {Email}", user.Email);

            if (_accountRepository.UserExists(user.Email, user.Phone))
            {
                _logger.LogError("Failed to create user account due to conflict for email: {Email}", user.Email);
                throw new Exceptions.ConflictException("User with Email or Phone already exists");
            }
            _logger.LogDebug("Account validation passed. Creating user in the database for email: {Email}", user.Email);

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(user.Password, salt);

            User userAccount = _mapper.Map<User>(user);
            userAccount.PasswordSalt = salt; // Store the salt alongside the hashed password
            userAccount.Password = hashedPassword;

            _repositoryWrapper.Account.Create(userAccount);
            _repositoryWrapper.Save();//Saving changes to database
            _logger.LogInformation("User created successfully with email: {Email}", user.Email);

        }
        public TokenDto Login(LoginDto login)
        {
            _logger.LogInformation("Attempting login for email: {Email}", login.Email);

            User? user = _repositoryWrapper.Account.FindByCondition(x => x.Email == login.Email).SingleOrDefault();
            if (user == null)
            {
                _logger.LogError($"Invalid user credentials,Given Email :{login.Email} is incorrect");
                throw new Exceptions.UnauthorizedException("Given Email is incorrect");
            }
            if (!VerifyPassword(login.Password, user.PasswordSalt, user.Password))
            {
                _logger.LogError("Invalid user credentials,Given Password is incorrect");
                throw new Exceptions.UnauthorizedException("Given password is incorrect");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration!["JWT:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("userid", user.Id.ToString()),
                    new Claim("role", user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("JWT token generated for user with email: {Email}", login.Email);
            return new TokenDto { Token = tokenHandler.WriteToken(token) };
        }

        /// <summary>
        /// The function generates a random salt of 16 bytes and returns it as a base64 encoded string.
        /// </summary>
        /// <returns>
        /// The method is returning a string which represents the generated salt.
        /// </returns>
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16]; // 16 bytes for the salt
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            string salt = Convert.ToBase64String(saltBytes);
            return salt;
        }

        /// <summary>
        /// This function takes a password and a salt, concatenates them, and then hashes the resulting byte array using SHA512 algorithm.
        /// </summary>
        /// <param name="password">The password that needs to be hashed.</param>
        /// <param name="salt">A randomly generated string of characters that is added to the password before hashing to increase security and prevent precomputed attacks. </param>
        /// <returns>
        /// The method is returning a string that represents the hashed password.
        /// </returns>
        private string HashPassword(string password, string salt)
        {
            using (var sha512 = SHA512.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltBytes = Convert.FromBase64String(salt);
                byte[] saltedPassword = new byte[passwordBytes.Length + saltBytes.Length];

                // Concatenate the salt and password
                saltBytes.CopyTo(saltedPassword, 0);
                passwordBytes.CopyTo(saltedPassword, saltBytes.Length);

                byte[] hashBytes = sha512.ComputeHash(saltedPassword);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashedPassword;
            }
        }

        /// <summary>
        /// The function verifies if a entered password matches a stored hashed password and salt.
        /// </summary>
        /// <param name="enteredPassword">The password entered by the user trying to log in.</param>
        /// <param name="storedSalt"></param>
        /// <param name="storedHashedPassword">The hashed password that is stored in the database or any other storage medium for a particular user account.</param>
        /// <returns> If they are equal, the method returns true, otherwise it returns false.
        /// </returns>
        private bool VerifyPassword(string enteredPassword, string storedSalt, string storedHashedPassword)
        {
            string hashedEnteredPassword = HashPassword(enteredPassword, storedSalt);
            return string.Equals(hashedEnteredPassword, storedHashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}