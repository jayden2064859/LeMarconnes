using ClassLibrary.Services;
using Xunit;

namespace TestProject.Unit
{
    public class CreateAccountServiceTests
    {
        // UTC-14: Valideer gebruikersnaam lengte (te kort)
        [Fact]
        public void ValidUsernameLength_TooShort_ReturnsFalse()
        {
            string username = "jan"; // 3 karakters
            bool result = CreateAccountService.ValidUsernameLength(username);
            Assert.False(result);
        }

        // UTC-15: Valideer gebruikersnaam lengte (geldig)
        [Fact]
        public void ValidUsernameLength_Valid_ReturnsTrue()
        {
            string username = "ValidUsername"; // 13 karakters
            bool result = CreateAccountService.ValidUsernameLength(username);
            Assert.True(result);
        }

        // UTC-16: Valideer gebruikersnaam lengte (te lang)
        [Fact]
        public void ValidUsernameLength_TooLong_ReturnsFalse()
        {
            string username = "User12345678901234567"; // 21 karakters
            bool result = CreateAccountService.ValidUsernameLength(username);
            Assert.False(result);
        }

        // UTC-17: Valideer gebruikersnaam karakters (ongeldig)
        [Fact]
        public void ValidUsernameChars_WithSpecialChars_ReturnsFalse()
        {
            string username = "user_123!";
            bool result = CreateAccountService.ValidUsernameChars(username);
            Assert.False(result);
        }

        // UTC-18: Valideer gebruikersnaam karakters (geldig)
        [Fact]
        public void ValidUsernameChars_OnlyLettersAndDigits_ReturnsTrue()
        {
            string username = "user123";
            bool result = CreateAccountService.ValidUsernameChars(username);
            Assert.True(result);
        }

        // UTC-19: Valideer gebruikersnaam verplicht veld
        [Fact]
        public void ValidateFields_EmptyUsername_ReturnsFalse()
        {
            string username = "";
            string password = "Test123";
            string confirmPassword = "Test123";

            bool result = CreateAccountService.ValidateFields(username, password, confirmPassword);
            Assert.False(result);
        }

        // UTC-20: Valideer gebruikersnaam lengte (maximum 20)
        [Fact]
        public void ValidUsernameLength_MaxLength_ReturnsTrue()
        {
            string username = "User1234567890123456"; // 20 karakters
            bool result = CreateAccountService.ValidUsernameLength(username);
            Assert.True(result);
        }

        // UTC-21: Valideer wachtwoord lengte (minimum)
        [Fact]
        public void ValidPasswordLength_Minimum_ReturnsTrue()
        {
            string password = "abcd"; // 4 karakters
            bool result = CreateAccountService.ValidPasswordLength(password);
            Assert.True(result);
        }

        // UTC-22: Valideer wachtwoord bevestiging (matching)
        [Fact]
        public void DoPasswordsMatch_MatchingPasswords_ReturnsTrue()
        {
            bool result = CreateAccountService.DoPasswordsMatch("Test123", "Test123");
            Assert.True(result);
        }

        // UTC-23: Valideer wachtwoord bevestiging (niet matching)
        [Fact]
        public void DoPasswordsMatch_NonMatchingPasswords_ReturnsFalse()
        {
            bool result = CreateAccountService.DoPasswordsMatch("Test123", "Test456");
            Assert.False(result);
        }

        // UTC-24: Valideer alle velden (geldige input)
        [Fact]
        public void ValidateFields_AllFieldsValid_ReturnsTrue()
        {
            bool result = CreateAccountService.ValidateFields(
                "testuser",
                "Test123",
                "Test123"
            );

            Assert.True(result);
        }

        // UTC-25: Valideer wachtwoord lengte (te kort)
        [Fact]
        public void ValidPasswordLength_TooShort_ReturnsFalse()
        {
            string password = "abc"; // 3 karakters
            bool result = CreateAccountService.ValidPasswordLength(password);
            Assert.False(result);
        }
    }
}
