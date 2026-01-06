using ClassLibrary.Services;
using Xunit;

namespace TestProject.Unit
{
    public class LoginValidationsTests
    {
        
        // UTC-26: Geldige login input
        [Fact]
        public void RequiredFields_ValidInput_ReturnsTrue()
        {
            // Arrange
            string username = "testuser";
            string password = "password123";

            // Act
            bool result = LoginValidation.RequiredFields(username, password);

            // Assert
            Assert.True(result);
        }

        // UTC-27: Lege gebruikersnaam
        [Fact]
        public void RequiredFields_EmptyUsername_ReturnsFalse()
        {
            // Arrange
            string username = "";
            string password = "password123";

            // Act
            bool result = LoginValidation.RequiredFields(username, password);

            // Assert
            Assert.False(result);
        }

        // UTC-28: Leeg wachtwoord
        [Fact]
        public void RequiredFields_EmptyPassword_ReturnsFalse()
        {
            // Arrange
            string username = "testuser";
            string password = "";

            // Act
            bool result = LoginValidation.RequiredFields(username, password);

            // Assert
            Assert.False(result);
        }

        // UTC-29: Beide velden leeg
        [Fact]
        public void RequiredFields_BothEmpty_ReturnsFalse()
        {
            // Arrange
            string username = "";
            string password = "";

            // Act
            bool result = LoginValidation.RequiredFields(username, password);

            // Assert
            Assert.False(result);
        }

        // UTC-31: Gebruikersnaam null
        [Fact]
        public void RequiredFields_NullUsername_ReturnsFalse()
        {
            // Arrange
            string username = null;
            string password = "password123";

            // Act
            bool result = LoginValidation.RequiredFields(username, password);

            // Assert
            Assert.False(result);
        }

        // UTC-32: Wachtwoord null
        [Fact]
        public void RequiredFields_NullPassword_ReturnsFalse()
        {
            // Arrange
            string username = "testuser";
            string password = null;

            // Act
            bool result = LoginValidation.RequiredFields(username, password);

            // Assert
            Assert.False(result);
        }

    }
}