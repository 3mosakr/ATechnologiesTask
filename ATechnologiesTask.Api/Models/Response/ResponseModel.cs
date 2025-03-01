using System.Net;

namespace PropertyApplication.Core.Base
{
    public class ResponseModel<T>
    {

        // status
        public HttpStatusCode StatusCode { get; set; }
        public bool Status { get; set; }

        public string? Message { get; set; }

        // data or errors
        public List<T>? Data { get; set; }
        public List<string>? Errors { get; set; }


        // Constructor(s)
        public ResponseModel()
        {
            Status = true;
            StatusCode = HttpStatusCode.OK;
        }
        public ResponseModel(List<T> data, string? message = null) : this()
        {
            Data = data;
            Message = message;
        }

        public ResponseModel(string message) : this()
        {
            Message = message;
        }

        public ResponseModel(string message, bool succeeded, List<string> errors = null)
        {
            StatusCode = succeeded ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            Status = succeeded;
            Message = message;
            Errors = errors;
        }
    }
}
