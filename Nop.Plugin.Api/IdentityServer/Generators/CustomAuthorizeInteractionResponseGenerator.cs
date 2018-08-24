
using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Api.IdentityServer.Generators
{
    public class NopApiAuthorizeInteractionResponseGenerator : IAuthorizeInteractionResponseGenerator
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        ///     The clock
        /// </summary>
        protected readonly ISystemClock Clock;

        /// <summary>
        ///     The consent service.
        /// </summary>
        protected readonly IConsentService Consent;

        /// <summary>
        ///     The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        ///     The profile service.
        /// </summary>
        protected readonly IProfileService Profile;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthorizeInteractionResponseGenerator" /> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="consent">The consent.</param>
        /// <param name="profile">The profile.</param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="authenticationService"></param>
        public NopApiAuthorizeInteractionResponseGenerator(
            ISystemClock clock,
            ILogger<AuthorizeInteractionResponseGenerator> logger,
            IConsentService consent,
            IProfileService profile,
            IHttpContextAccessor httpContextAccessor,
            IAuthenticationService authenticationService)
        {
            Clock = clock;
            Logger = logger;
            Consent = consent;
            Profile = profile;
            _httpContextAccessor = httpContextAccessor;
            _authenticationService = authenticationService;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Processes the interaction logic.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="consent">The consent.</param>
        /// <returns></returns>
        public virtual async Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthorizeRequest request,
            ConsentResponse consent = null)
        {
            Logger.LogTrace("ProcessInteractionAsync");

            if (consent != null && consent.Granted == false && request.Subject.IsAuthenticated() == false)
            {
                // special case when anonymous user has issued a deny prior to authenticating
                Logger.LogInformation("Error: User denied consent");
                return new InteractionResponse
                {
                    Error = OidcConstants.AuthorizeErrors.AccessDenied
                };
            }

            var identityServerUser = new IdentityServerUser(request.ClientId)
            {
                DisplayName = request.Client.ClientName,
                AdditionalClaims = request.ClientClaims,
                AuthenticationTime = DateTime.UtcNow
            };

            request.Subject = identityServerUser.CreatePrincipal();

            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow
            };

            await _authenticationService.SignInAsync(_httpContextAccessor.HttpContext,
                IdentityServerConstants.DefaultCookieAuthenticationScheme, request.Subject, authenticationProperties);
        

            return new InteractionResponse();
        }
    }
}