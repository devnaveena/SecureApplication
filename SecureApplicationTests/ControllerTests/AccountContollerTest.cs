using Entities.Dtos.RequestDto;
using Entities.Dtos.ResponseDto;
using SecureApplication;
using Entities;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using SecureApplicationTests.Helpers;
namespace SecureApplicationTests.ControllerTests
{

    public class AccountControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private readonly RepositoryContext _context; // Existing in-memory database context

        public AccountControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _context = _factory.Services.GetRequiredService<RepositoryContext>();
            _client = _factory.CreateClient();
        }


        /// <summary>
        /// This function tests that creating a user with an email or phone number that already exists
        /// throws a ConflictException.
        /// </summary>
        [Fact]
        public void CreateUserAccount_UserExists_ThrowsConflictException()
        {
            // Arrange
            var user = new UserDto
            {
                // Set the user properties with existing email 
                UserName = "TestUser",
                Password = "TestPassword",
                DateOfBirth = new DateTime(1990, 1, 1),
                Email = "navee@example.com",
                Phone = 1234567890,
                Role = "Admin",
                Gender = "Male",
            };

            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act and Assert
            var response = _client.PostAsync("/api/user", content).Result;

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        /// <summary>
        /// The function tests if creating a user account with invalid data returns a bad request error.
        /// </summary>
        [Fact]
        public void CreateUserAccount_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var user = new UserDto
            {
                // Set the user properties with invalid role 
                UserName = "Naveena",
                Password = "TestPassword",
                DateOfBirth = new DateTime(2025, 1, 1),
                Email = "test@example.com",
                Phone = 1234567890,
                Role = "Admin",//Invalid Role
                Gender = "Male",
            };

            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = _client.PostAsync("/api/user", content).Result;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }
        /// <summary>
        /// This function tests if creating a user account with valid data returns a status code of 201
        /// (created).
        /// </summary>
        [Fact]
        public void CreateUserAccount_ValidData_ReturnsCreatedStatus()
        {
            // Arrange
            var user = new UserDto
            {
                // Set the user properties with valid data
                UserName = "TestUser",
                Password = "TestPassword",
                DateOfBirth = new DateTime(1990, 1, 1),
                Email = "test@example.com",
                Phone = 1234567890,
                Role = "Admin",
                Gender = "Male",
            };

            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = _client.PostAsync("/api/user", content).Result;

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        /// <summary>
        /// This function tests the login functionality for an admin user and returns a token if the
        /// login is successful.
        /// </summary>
        /// <returns>
        /// The method is returning a string which is the token obtained after successfully logging in
        /// with valid data for an admin user.
        /// </returns>
        [Fact]
        public string LoginForAdmin_validData_ReturnsToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "navee@example.com",
                Password = "Navee@2002",
            };

            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = _client.PostAsync("/api/user/login", content).Result;

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync();
            TokenDto? tokenDto = JsonConvert.DeserializeObject<TokenDto>(responseContent.Result);
            Assert.NotNull(tokenDto);

            return tokenDto!.Token;
        }

        /// <summary>
        /// The function tests if a valid user login returns a token.
        /// </summary>
        /// <returns>
        /// The method is returning a string which is the token obtained after successfully logging in
        /// with valid user data.
        /// </returns>
        [Fact]
        public string LoginForUser_validData_ReturnsToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "Abi@example.com",
                Password = "Abi@2002",
            };

            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = _client.PostAsync("/api/user/login", content).Result;

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync();
            TokenDto? tokenDto = JsonConvert.DeserializeObject<TokenDto>(responseContent.Result);
            Assert.NotNull(tokenDto);

            return tokenDto!.Token;
        }

        /// <summary>
        /// The function tests that invalid login data returns a bad request with a specific error message.
        /// </summary>
        [Fact]
        public void Login_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "testexample.com",//Invalid- email
                Password = "TestPassword",
            };

            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = _client.PostAsync("/api/user/login", content).Result;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// This function tests that attempting to log in with an invalid email address throws an
        /// UnauthorizedException with the message "Given Email is incorrect".
        /// </summary>
        [Fact]
        public void Login_InvalidEmail_ThrowsUnauthorizedException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "TestPassword",
            };

            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = _client.PostAsync("/api/user/login", content).Result;

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);


        }

        /// <summary>
        /// This function tests that attempting to log in with an invalid password throws an
        /// UnauthorizedException.
        /// </summary>
        [Fact]
        public void Login_InvalidPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "navee@example.com",
                Password = "TestPassword",
            };

            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = _client.PostAsync("/api/user/login", content).Result;

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var responseContent = response.Content.ReadAsStringAsync();
            ErrorDto? errorDto = JsonConvert.DeserializeObject<ErrorDto>(responseContent.Result);

            Assert.Equal("Given password is incorrect", errorDto!.Description);
        }

    }
}