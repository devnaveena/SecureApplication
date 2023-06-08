using System.Net;
using Entities.Dtos.ResponseDto;
using Exceptions;
using Newtonsoft.Json;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ConflictException ex)
        {
            _logger.LogError(ex, "ConflictException occurred: " + ex.Message);
            await HandleExceptionAsync("Conflict", context, 409, ex.Message);
        }
        catch (BadRequestException ex)
        {
            _logger.LogError(ex, "BadRequestException occurred: " + ex.Message);
            await HandleExceptionAsync("Badrequest", context, 400, ex.Message);
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, "NotFoundException occurred: " + ex.Message);
            await HandleExceptionAsync("NotFound", context, 404, ex.Message);
        }
        catch (NoContentException ex)
        {
            _logger.LogError(ex, "NoContentException occurred: " + ex.Message);
            await HandleExceptionAsync("NoContent", context, 204, ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogError(ex, "UnauthorizedException occurred: " + ex.Message);
            await HandleExceptionAsync("UnauthorizedAccessException", context, 401, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            _logger.LogError(ex, "Unauthorized access: " + ex.Message);
            await HandleExceptionAsync("Unauthorized access", context, 403, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error  occurred: " + ex.Message);
            await HandleExceptionAsync("InternalServer", context, 500, ex.Message);
        }

    }
    /// <summary>
    /// This function handles exceptions and returns a consistent error response in JSON format.
    /// </summary>
    /// <param name="error">A string representing the error message to be returned in the error
    /// response.</param>
    /// <param name="HttpContext"></param>
    /// <param name="statusCode">an integer representing the HTTP status code to be returned in the response.</param>
    /// <param name="description"></param>
    private static async Task HandleExceptionAsync(string error, HttpContext context, int statusCode, string description)
    {
        // Handle the exception and return a consistent error response
        var errorDto = new ErrorDto
        {
            ErrorMessage = error,
            StatusCode = statusCode,
            Description = description
        };

        // Set the response status code
        context.Response.StatusCode = (int)errorDto.StatusCode;

        // Write the error response as JSON
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorDto));
    }

}
