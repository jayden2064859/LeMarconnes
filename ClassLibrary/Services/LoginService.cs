using ClassLibrary.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Services
{
    public class LoginService
    {
        public static LoginDTO CreateNewLoginDTO(string username, string password)
        {
            return new LoginDTO
            {
                Username = username,
                Password = password
            };
        }
        public static bool RequiredFields(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            return true;
        }
    }
}

