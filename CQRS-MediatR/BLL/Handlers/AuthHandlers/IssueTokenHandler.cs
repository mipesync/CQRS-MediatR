using CQRS_MediatR.API;
using CQRS_MediatR.BLL.Commands;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace CQRS_MediatR.BLL.Handlers.UserHandlers
{
    public class IssueTokenHandler : IRequestHandler<IssueTokenCommand, string>
    {
        public Task<string> Handle(IssueTokenCommand request, CancellationToken cancellationToken)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, request.User.Name!), new Claim(ClaimTypes.GivenName, request.User.Username) };

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(20)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            request.Context.Session.SetString("access_token", encodedJwt);
            request.Context.Session.SetString("userId", request.User.Id);
            request.Context.Response.Cookies.Append("access_token", encodedJwt);
            request.Context.Response.Headers["access_token"] = encodedJwt;

            return Task.FromResult(encodedJwt);
        }
    }
}
