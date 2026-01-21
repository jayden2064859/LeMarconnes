using API.Services;
using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.DbServices
{
    public class AuthenticateDbService
    {
        private readonly LeMarconnesDbContext _context;
        private readonly JwtService _jwtService; // voor token generatie

        public AuthenticateDbService(LeMarconnesDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // het meegegeven wachtwoord wordt gehashed en vergeleken met de opgeslagen hash van het matchende account in de database
        public async Task<(AuthenticateResponseDTO? responseDto, string? errorMessage)> AuthenticateAsync(AuthenticateDTO dto)
        {
            // account ophalen 
            var (account, errorMessage) = await GetAccountByUsernameAsync(dto.Username);

            if (account == null)
            {
                return (null, errorMessage);
            }

            // ingevoerde wachtwoord uit dto halen, hashen en vergelijken met opgeslagen hash van het account
            var hasher = new PasswordHasher<Account>();
            var result = hasher.VerifyHashedPassword(null, account.PasswordHash, dto.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return (null, "Ongeldige inloggegevens"); 
            }

            // token genereren
            var token = GetUserToken(account);

            // private methode om response DTO aan te maken en terug geven aan controller
            var responseDto = CreateResponseDto(account, token);
            return (responseDto, null);
        }


        private async Task<(Account? account, string? errorMessage)> GetAccountByUsernameAsync(string username)
        {
            // zoek account op basis van gebruikersnaam
            var account = await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Username == username);

            if (account == null)
            {
                return (null, "Gebruikersnaam bestaat niet"); // return errormessage, geen account object
            }
            return (account, null); // return account object, geen errormessage
        }

        // jwt service gebruiken om token te genereren
        private string GetUserToken(Account account)
        {
            var token = _jwtService.GenerateToken(account);
            return token;
        }

        private AuthenticateResponseDTO CreateResponseDto(Account account, string token)
        {
            var dto = new AuthenticateResponseDTO
            {
                Username = account.Username,
                Role = account.AccountRole,
                CustomerId = account.CustomerId,
                FirstName = account.Customer?.FirstName,
                Token = token
            };
            return dto;
        }

    }
}
