using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Nop.Plugin.Api.Controllers.OAuth
{
    public class OAuthController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Authorize(string client_id)
        {
            if (HttpContext.Current.Response.StatusCode != 200)
            {
                return BadRequest();
            }

            var authentication = HttpContext.Current.GetOwinContext().Authentication;

            var identity = new ClaimsIdentity("Bearer");

            identity.AddClaims(new List<Claim>()
            {
                new Claim("client_id", client_id)
            });

            authentication.SignIn(identity);

            return Ok();
        }
    }
}
