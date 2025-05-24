using Newtonsoft.Json;

namespace TaskFlow.Models
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public int HttpStatusCode { get; set; }
        public string Exception { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
