using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace authz
{
    public class DevAuthAuthenticationSchemeOptions
        : AuthenticationSchemeOptions
    { }

    public class DevAuthAuthenticationHandler
        : AuthenticationHandler<DevAuthAuthenticationSchemeOptions>
    {
        public DevAuthAuthenticationHandler(
            IOptionsMonitor<DevAuthAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        const string HeaderName = "X-DevAuth-UserId";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(HeaderName))
            {
                return Task.FromResult(AuthenticateResult.Fail($"No {HeaderName} found."));
            }

            var userId = Request.Headers[HeaderName].ToString();
            var claims = new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "DevAuth");
            var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(claimsIdentity), 
                    this.Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}