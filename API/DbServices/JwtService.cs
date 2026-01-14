using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.DbServices
{
    // de jwt service is bedoeld voor API authenticatie. Pas wanneer een gebruiker ingelogd is, mag hij toegang tot bepaalde
    // api endpoints, zodat niet alles publiekelijk bereikbaar is
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // method voor authenticatie token genereren per user
        public string GenerateToken(Account account)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]);
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, account.Username),
            new Claim(ClaimTypes.Role, account.AccountRole.ToString()),
            new Claim("CustomerId", account.CustomerId?.ToString() ?? "")

        };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtConfig:Issuer"],
                audience: _configuration["JwtConfig:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["JwtConfig:TokenValidityMins"] ?? "30")),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}