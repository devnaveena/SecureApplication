using Entities.Dtos.ResponseDto;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Contracts.IServices
{
    public interface IService
    {
        ///<summary>
        /// Validates user entered data 
        ///</summary>
        ///<return>ErrorDTO with the specified errors</return>
        public void ModelStateInvalid(ModelStateDictionary ModelState);

    }
}