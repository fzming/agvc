using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CoreService.JwtToken
{
    public class JwtTokenOptions
    {
        public string Issuer { get; set; }

        public bool ValidateIssuer { get; set; }
        public string Audience { get; set; }
        public bool ValidateAudience { get; set; }
        public string RawSigningKey { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool RequireExpirationTime { get; set; }
        public int JwtExpiresInMinutes { get; set; }

        public int AccessTokenExpiresMinutes { get; set; }
        public bool ValidateIntervaltime { get; set; }
        public int IntervalExpiresInMinutes { get; set; }

        public TokenValidationParameters ToTokenValidationParams()
        {
            return new()
            {
                // The signing key must match!
                ValidateIssuerSigningKey = ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RawSigningKey)),

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = ValidateIssuer,
                ValidIssuer = Issuer,

                // Validate the JWT Audience (aud) claim
                ValidateAudience = ValidateAudience,
                ValidAudience = Audience,

                // Validate the token expiry
                ValidateLifetime = ValidateLifetime,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}