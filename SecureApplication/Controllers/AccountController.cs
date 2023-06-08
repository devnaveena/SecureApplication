using Microsoft.AspNetCore.Mvc;
using Contracts.IServices;
using Microsoft.AspNetCore.Authorization;
using Entities.Dtos.RequestDto;
using Entities.Dtos.ResponseDto;
using Exceptions;
using System.Net;

namespace SecureApplication.Controllers
{

    [ApiController]
    [Route("api/user")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        private readonly IService _service;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger, IService service)
        {
            _accountService = accountService;
            _logger = logger;
            _service = service;
        }


        /// <summary>
        /// This function creates a user account and returns a status code indicating Created
        /// </summary>
        /// <param name="user">Dto  contains properties such as username, email, password, and other relevant user information.</param>
        /// <returns>
        /// The method is returning a `StatusCodeResult` with a status code of `201 Created`.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateUserAccount([FromBody] UserDto user)
        {
            if (!ModelState.IsValid)
            {
                // If Model validation failed, return 400 Bad Request with validation errors
                _logger.LogWarning("Model validation failed while creating user account");
                _service.ModelStateInvalid(ModelState);
            }
            _accountService.CreateUser(user);
            return StatusCode(StatusCodes.Status201Created);
        }


        /// <summary>
        /// This  handles a login request, validates the input, logs the successful login, and returns an access token.
        /// </summary>
        /// <param name="logIn">Dto that contains the login credentials of a user, such as their email and password.</param>
        /// <returns>
        /// An IActionResult object is being returned, which contains an Ok result with a TokenDto object (accessToken) as its content.
        /// </returns>

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDto logIn)
        {
            if (!ModelState.IsValid)
            {
                // If Model validation failed, return 400 Bad Request with validation errors
                _logger.LogWarning("Model validation failed while logging in.");
                _service.ModelStateInvalid(ModelState);
            }

            TokenDto accessToken = _accountService.Login(logIn);
            _logger.LogInformation($"Login Sucessfull for the user : {logIn.Email}");
            return Ok(accessToken);
        }
    }
}