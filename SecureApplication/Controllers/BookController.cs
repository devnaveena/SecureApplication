using Contracts.IServices;
using Entities.Dtos.RequestDto;
using Entities.Dtos.ResponseDto;
using Entities.Helpher;
using Entities.Models;
using Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureApplication.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api/book")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;
        private readonly IService _service;

        public BookController(IBookService bookService, ILogger<BookController> logger, IService service)
        {
            _bookService = bookService;
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// This function creates a book and returns a 201 status code if the user is an admin and the bookDto is valid.
        /// </summary>
        /// <param name="bookDto"> Dto that contains properties such as title, author, and ISBN.</param>
        /// <returns>
        /// The method is returning a HTTP status code 201 (Created) if the book is created successfully.
        /// </returns>
        [HttpPost]
        [CustomAuthorize(Role = "Admin")]
        public IActionResult CreateBook([FromBody] BookDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                //If Model validation failed, return 400 Bad Request with validation errors
                _service.ModelStateInvalid(ModelState);
            }
            _bookService.CreateBook(bookDto);
            _logger.LogInformation("Book Created Sucessfully");
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// This function returns a list of books based on the user's access level, page number, page
        /// size, and filter value.
        /// </summary>
        /// <param name="pageNumber">The page number of the results to be returned. Defaults to 1 if not provided.</param>
        /// <param name="pageSize">The number of items to be displayed on a single page of the result set.</param>
        /// <param name="filterValue">This is an optional parameter that allows the user to filter the list of books based on a specific value.</param>
        /// <returns>
        /// The method returns a list of books as a response based on the access level of the user
        /// </returns>
        [HttpGet]
        [CustomAuthorize(Role = "Admin,Reader,User")]
        public IActionResult GetAllBooks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filterValue = null)
        {
            if (User.IsInRole("Reader"))
            {

                List<ReaderDto> readerbookList = _bookService.GetAllBooks<ReaderDto>(pageNumber, pageSize, filterValue);
                // return List of books as response based on the Access
                return Ok(readerbookList);
            }

            List<AdminDto> adminbookList = _bookService.GetAllBooks<AdminDto>(pageNumber, pageSize, filterValue);
            // return List of books as response based on the Access
            return Ok(adminbookList);
        }

        /// <summary>
        /// This function returns book details for a given ID based on the user's access level.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// The method returns the book details for the given Id as a response based on the access level of
        /// the user. If the user is an admin, it returns the book details as an AdminDto object, otherwise
        /// it returns the book details as a ReaderDto object. The response is returned as an IActionResult
        /// object with an HTTP status code of 200 (OK).
        /// </returns>
        [HttpGet("{id}")]
        [CustomAuthorize(Role = "Admin,Reader,User")]
        public IActionResult GetBookById(Guid id)
        {
            if (User.IsInRole("Reader"))
            {
                ReaderDto readerbooks = _bookService.GetBookById<ReaderDto>(id);
                _logger.LogInformation($"Returned the book with this {id}");
                return Ok(readerbooks);// returns the book details for the given Id  as response based on the Access
            }

            AdminDto adminbooks = _bookService.GetBookById<AdminDto>(id);
            _logger.LogInformation($"Returned the book with this {id}");
            return Ok(adminbooks); // returns the book details for the given Id  as response based on the Access
        }

        /// <summary>
        /// This function deletes a book with a specified ID and returns a 403 Forbidden status if the user is not an admin.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// The DeleteBook method is returning an IActionResult, which in this case is an Ok result with a 200 status code.
        /// </returns>
        [HttpDelete("{id}")]
        [CustomAuthorize(Role = "Admin")]
        public IActionResult DeleteBook(Guid id)
        {
            _bookService.DeleteBook(id);
            _logger.LogInformation($"Book with ID {id} deleted successfully");
            return Ok();//Return Success if the book deleted successfully
        }

    }

}
