using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Api.JSON.ActionResults
{
    public class ErrorActionResult : IActionResult 
    {
        private readonly string _jsonString;
        private readonly HttpStatusCode _statusCode;

        public ErrorActionResult(string jsonString, HttpStatusCode statusCode)
        {
            _jsonString = jsonString;
            _statusCode = statusCode;
        }
        
        public Task ExecuteResultAsync(ActionContext context)
        {
            var content = new StringContent(_jsonString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = new HttpResponseMessage(_statusCode) { Content = content };
            return Task.FromResult(response);
        }
    }
}