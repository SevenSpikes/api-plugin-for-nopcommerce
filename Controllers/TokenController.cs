using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Configuration;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.Models.Authentication;
using Nop.Services.Customers;
using Nop.Services.Logging;

namespace Nop.Plugin.Api.Controllers
{
    [AllowAnonymous]
    public class TokenController : Controller
    {
        private readonly ApiConfiguration _apiConfiguration;
        private readonly ApiSettings _apiSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;

        public TokenController(
            ICustomerService customerService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerActivityService customerActivityService,
            CustomerSettings customerSettings,
            ApiSettings apiSettings,
            ApiConfiguration apiConfiguration)
        {
            _customerService = customerService;
            _customerRegistrationService = customerRegistrationService;
            _customerActivityService = customerActivityService;
            _customerSettings = customerSettings;
            _apiSettings = apiSettings;
            _apiConfiguration = apiConfiguration;
        }

        [Route("/token")]
        [HttpGet]
        public IActionResult Create(TokenRequest model)
        {
            if (string.IsNullOrEmpty(model.Username))
            {
                return Json(new TokenResponse("Missing username"));
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return Json(new TokenResponse("Missing password"));
            }

            var customer = ValidateUser(model);

            if (customer != null)
            {
                return Json(GenerateToken(customer));
            }

            return Forbid();// new HttpStatusCodeResult(HttpStatusCode.Forbidden);// Json(new TokenResponse("Access Denied"));
        }

        private CustomerLoginResults LoginCustomer(TokenRequest model)
        {
            var loginResult = _customerRegistrationService
                .ValidateCustomer(model.Username, model.Password);

            return loginResult;
        }

        private Customer ValidateUser(TokenRequest model)
        {
            var result = LoginCustomer(model);

            if (result == CustomerLoginResults.Successful)
            {
                var customer = _customerSettings.UsernamesEnabled
                                   ? _customerService.GetCustomerByUsername(model.Username)
                                   : _customerService.GetCustomerByEmail(model.Username);


                //activity log
                _customerActivityService.InsertActivity(customer, "Api.TokenRequest", "User API token request", customer);

                return customer;
            }

            return null;
        }

        private int GetTokenExpiryInDays()
        {
            return _apiSettings.TokenExpiryInDays <= 0
                       ? Constants.Configurations.DefaultAccessTokenExpirationInDays
                       : _apiSettings.TokenExpiryInDays;
        }

        private TokenResponse GenerateToken(Customer customer)
        {
            var expiresInSeconds = new DateTimeOffset(DateTime.Now.AddDays(GetTokenExpiryInDays())).ToUnixTimeSeconds();

            var claims = new List<Claim>
                         {
                             new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                             new Claim(JwtRegisteredClaimNames.Exp, expiresInSeconds.ToString()),
                             new Claim(ClaimTypes.Email, customer.Email),
                             new Claim(ClaimTypes.NameIdentifier, customer.CustomerGuid.ToString()),
                             _customerSettings.UsernamesEnabled
                                 ? new Claim(ClaimTypes.Name, customer.Username)
                                 : new Claim(ClaimTypes.Name, customer.Email)
                         };

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiConfiguration.SecurityKey)),
                                                            SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(new JwtHeader(signingCredentials), new JwtPayload(claims));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);


            return new TokenResponse(accessToken, expiresInSeconds);
        }
    }
}
