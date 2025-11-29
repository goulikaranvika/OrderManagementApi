
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderManagementApi.Models;

namespace OrderManagementApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> CreateToken(ApplicationUser user)
        {
            // Read JWT settings from appsettings.json
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var durationInMinutes = double.Parse(jwtSection["DurationInMinutes"] ?? "60");

            // Claims that will be inside the JWT token
            var claims = new List<Claim>
            {
                // Unique user id – used in OrdersController as ClaimTypes.NameIdentifier
                new Claim(ClaimTypes.NameIdentifier, user.Id),

                // Email – also mapped to User.Identity.Name (since we use email login)
                new Claim(ClaimTypes.Name, user.Email ?? string.Empty),

                // Optional: explicitly store email claim too
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            // Create signing key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create token descriptor
            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationInMinutes),
                signingCredentials: credentials
            );

            // Write token to string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenDescriptor);

            return Task.FromResult(token);
        }
    }
}
