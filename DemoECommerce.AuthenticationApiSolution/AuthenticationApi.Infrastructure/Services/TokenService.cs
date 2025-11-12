using AuthenticationApi.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Services;

internal class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private byte[] _jwtKeyInBytes;
    public TokenService(IConfiguration config)
    {
        _config = config;
        _jwtKeyInBytes = Encoding.UTF8.GetBytes(_config["Authentication:Key"]!);
    }

    public string GenerateToken(string userId, string email, string role)
    {
        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

        var key = new SymmetricSecurityKey(_jwtKeyInBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Authentication:Issuer"],
            audience: _config["Authentication:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(_jwtKeyInBytes);
            var handler = new JwtSecurityTokenHandler();

            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
