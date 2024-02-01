using System.Net;

namespace CouponApi.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            var ErrorMessages = new List<string>();
        }
        public bool IsSuccess { get; set; }
        public Object Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
