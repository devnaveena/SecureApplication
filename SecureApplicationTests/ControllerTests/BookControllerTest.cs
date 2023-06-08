using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Entities;
using Entities.Dtos.RequestDto;
using Entities.Dtos.ResponseDto;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SecureApplicationTests.Helpers;
using SecureApplication;

namespace SecureApplicationTests.ControllerTests
{

    public class BookControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private readonly AccountControllerTest _accountControllerTest;
        private readonly RepositoryContext _context; // Existing in-memory database context

        public BookControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _context = _factory.Services.GetRequiredService<RepositoryContext>();
            _client = _factory.CreateClient();
            _accountControllerTest = new AccountControllerTest(_factory);
        }

        /// <summary>
        /// This function tests if creating a book with valid book data returns a created status code.
        /// </summary>
        [Fact]
        public void CreateBook_WithValidBookDto_ReturnsCreatedStatusCode()
        {
            // Arrange
            var token = _accountControllerTest.LoginForAdmin_validData_ReturnsToken();

            var bookDto = new BookDto
            {
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                Description = "A classic American novel depicting the lavish and decadent lifestyle of the 1920s.",
                PublishDate = new DateTime(1925, 4, 10),
                IsbnNumber = "978-167883456700",
                Publisher = "Scribner",
                Location = "New York",
                Language = "English",
                Genre = "Fiction",
                PageCount = 180,
                Price = 12.99m,
                IsAvailable = true
            };

            var json = JsonConvert.SerializeObject(bookDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = _client.PostAsync("/api/book", content).Result;

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);


        }
        /// <summary>
        /// This function tests that attempting to create a book with a reader role results in a
        /// Forbidden exception.
        /// </summary>
        [Fact]
        public void CreateBook_WithReaderRole_ThrowsForbiddenException()
        {
            // Arrange
            var token = _accountControllerTest.LoginForUser_validData_ReturnsToken();

            var bookDto = new BookDto
            {
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                Description = "A classic American novel depicting the lavish and decadent lifestyle of the 1920s.",
                PublishDate = new DateTime(1925, 4, 10),
                IsbnNumber = "978-167883456780",
                Publisher = "Scribner",
                Location = "New York",
                Language = "English",
                Genre = "Fiction",
                PageCount = 180,
                Price = 12.99m,
                IsAvailable = true
            };

            var json = JsonConvert.SerializeObject(bookDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = _client.PostAsync("/api/book", content).Result;

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);


        }

        /// <summary>
        /// This function tests that attempting to create a book with an existing ISBN number results in
        /// a Conflict HTTP status code.
        /// </summary>
        [Fact]
        public void CreateBook_BookwithIsbnNumberAlreadyExists_ThrowsConflictException()
        {
            // Arrange
            var token = _accountControllerTest.LoginForAdmin_validData_ReturnsToken();

            var bookDto = new BookDto
            {
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                Description = "A classic American novel depicting the lavish and decadent lifestyle of the 1920s.",
                PublishDate = new DateTime(1925, 4, 10),
                IsbnNumber = "978-167883456780",
                Publisher = "Scribner",
                Location = "New York",
                Language = "English",
                Genre = "Fiction",
                PageCount = 180,
                Price = 12.99m,
                IsAvailable = true
            };

            var json = JsonConvert.SerializeObject(bookDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = _client.PostAsync("/api/book", content).Result;

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);


        }

        /// <summary>
        /// The function tests if creating a book with invalid data returns a bad request status code.
        /// </summary>
        [Fact]
        public void CreateBook_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var token = _accountControllerTest.LoginForAdmin_validData_ReturnsToken();

            var bookDto = new BookDto
            {
                Title = "",
                Author = "F. Scott Fitzgerald",
                Description = "A classic American novel depicting the lavish and decadent lifestyle of the 1920s.",
                PublishDate = new DateTime(1925, 4, 10),
                IsbnNumber = "978-167883456780",
                Publisher = "Scribner",
                Location = "New York",
                Language = "English",
                Genre = "Fiction",
                PageCount = 180,
                Price = 0,
                IsAvailable = true
            };

            var json = JsonConvert.SerializeObject(bookDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = _client.PostAsync("/api/book", content).Result;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);


        }
        /// <summary>
        /// This function tests the GetAllBooks method in a web API by checking if it returns a list of
        /// books based on the user's role.
        /// </summary>
        [Fact]
        public void GetAllBooks_ReturnsListOfBooksBasedOnUserRole()
        {
            // Arrange 
            var token = _accountControllerTest.LoginForAdmin_validData_ReturnsToken();

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = _client.GetAsync("/api/book").Result;
            var responseBody = response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var adminBookList = JsonConvert.DeserializeObject<List<AdminDto>>(responseBody.Result);
            Assert.NotNull(adminBookList);
            //  assert for the AdminDto list

            // Validation for User
            var tokenForUser = _accountControllerTest.LoginForUser_validData_ReturnsToken();

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenForUser);

            // Act
            var responseforUser = _client.GetAsync("/api/book").Result;
            var responseBodyUser = response.Content.ReadAsStringAsync();
            var readerBookList = JsonConvert.DeserializeObject<List<ReaderDto>>(responseBodyUser.Result);
            Assert.NotNull(readerBookList);
            //  assertions for the ReaderDto list
        }
        /// <summary>
        /// The function tests whether the GetBookById method returns the correct book based on the
        /// user's role.
        /// </summary>
        [Fact]
        public void GetBookById_ReturnsBookBasedOnUserRole()
        {
            // Arrange
            var token = _accountControllerTest.LoginForAdmin_validData_ReturnsToken();

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Act
            var response = _client.GetAsync($"/api/book/0B5851A4-02CA-4370-8D1B-130C36D369CA").Result;
            var responseBody = response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var adminBook = JsonConvert.DeserializeObject<AdminDto>(responseBody.Result);
            Assert.NotNull(adminBook);
            // Additional assertions for the AdminDto book

            // Validation for User
            var tokenForUser = _accountControllerTest.LoginForUser_validData_ReturnsToken();

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenForUser);

            // Act
            var responseforUser = _client.GetAsync($"/api/book/0B5851A4-02CA-4370-8D1B-130C36D369CA");
            var responseBodyUser = response.Content.ReadAsStringAsync();
            var readerBook = JsonConvert.DeserializeObject<ReaderDto>(responseBodyUser.Result);
            Assert.NotNull(readerBook);
            // Additional assertions for the ReaderDto book

        }

        /// <summary>
        /// This function tests if attempting to get a book by an invalid ID throws a NotFoundException.
        /// </summary>
        [Fact]
        public void GetBookById_IfBookNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var token = _accountControllerTest.LoginForAdmin_validData_ReturnsToken();

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Act
            var response = _client.GetAsync($"/api/book/0B5851A4-02CB-4370-8D1B-130C36D369CA").Result;

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }

        /// <summary>
        /// This function tests if deleting a book with a valid ID returns an OK status code.
        /// </summary>
        [Fact]
        public void DeleteBook_WithValidId_ReturnsOkStatusCode()
        {
            // Arrange
            var token = _accountControllerTest.LoginForAdmin_validData_ReturnsToken();


            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = _client.DeleteAsync($"/api/book/0B5851A4-02CA-4370-8D1B-130C36D369CA").Result;

            // Act
            var responseforNoContent = _client.GetAsync("/api/book").Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseforNoContent.StatusCode);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        /// <summary>
        /// This function tests that attempting to delete a book that does not exist will throw a
        /// NotFoundException.
        /// </summary>
        [Fact]
        public void DeleteBook_IfBookNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var token = _accountControllerTest.LoginForAdmin_validData_ReturnsToken();

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = _client.DeleteAsync($"/api/book/0B5851A4-02CA-4370-8D1G-130C36D369CA").Result;


            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// This function tests if deleting a book with a reader role throws a Forbidden exception.
        /// </summary>
        [Fact]
        public void DeleteBook_IfReaderRole_ThrowsForbiddenException()
        {
            // Arrange
            var token = _accountControllerTest.LoginForUser_validData_ReturnsToken();

            // Set the authorization token in the request headers
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = _client.DeleteAsync($"/api/book/0B5851A4-02CA-4370-8D1G-130C36D369CA").Result;


            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }

}