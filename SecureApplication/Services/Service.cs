using Contracts.IServices;
using Entities.Dtos.ResponseDto;
using Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Services
{
    public class Service : IService
    {

        public void ModelStateInvalid(ModelStateDictionary ModelState)
        {
            ErrorDto errorDto = new ErrorDto
            {
                ErrorMessage = ModelState.Keys.FirstOrDefault(),
                Description = ModelState.Values.Select(src => src.Errors.Select(src => src.ErrorMessage).FirstOrDefault()).FirstOrDefault() ?? "BadRequest",
                StatusCode = 400
            };
            throw new Exceptions.BadRequestException(errorDto.Description);
        }
    }
}