using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CQRS_MediatR.API
{
    public class AuthOptions
    {
        public static string ISSUER = "Alferov Inc.";
        public static string AUDIENCE = "https://localhost:7277";
        const string KEY = "375ff8a55aca72cf3a11e318d1592d2f0d3995ae";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
