using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.Exceptions
{
    public class NopApiClientExceptionDetails
    {
        public string Content { get; set; }

        public string Message { get; set; }

        [JsonProperty("errors")] public dynamic Errors { get; set; }

        public string ExceptionType { get; set; }

        public string StackTrace { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}