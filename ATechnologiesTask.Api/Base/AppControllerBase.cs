using Microsoft.AspNetCore.Mvc;
using PropertyApplication.Core.Base;
using System.Net;

namespace ATechnologiesTask.Api.Base
{
    /// <summary>
    /// Base controller class providing common functionality for all API controllers.
    /// </summary>
    [ApiController]
    public class AppControllerBase : ControllerBase
    {

        #region Actions
        /// <summary>
        /// Creates an appropriate <see cref="ObjectResult"/> based on the status code of the response.
        /// </summary>
        /// <typeparam name="T">The type of the response model.</typeparam>
        /// <param name="response">The response model containing the status code and data.</param>
        /// <returns>An <see cref="ObjectResult"/> representing the result of the operation.</returns>
        public ObjectResult NewResult<T>(ResponseModel<T> response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return new OkObjectResult(response);
                case HttpStatusCode.Created:
                    return new CreatedResult(string.Empty, response);
                case HttpStatusCode.Unauthorized:
                    return new UnauthorizedObjectResult(response);
                case HttpStatusCode.BadRequest:
                    return new BadRequestObjectResult(response);
                case HttpStatusCode.Conflict:
                    return new ConflictObjectResult(response);
                case HttpStatusCode.NotFound:
                    return new NotFoundObjectResult(response);
                case HttpStatusCode.Accepted:
                    return new AcceptedResult(string.Empty, response);
                case HttpStatusCode.UnprocessableEntity:
                    return new UnprocessableEntityObjectResult(response);
                default:
                    return new BadRequestObjectResult(response);
            }
        }
        #endregion
    }
}
