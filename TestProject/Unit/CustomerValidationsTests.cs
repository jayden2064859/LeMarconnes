using ClassLibrary.Services;
using Xunit;

namespace TestProject.Unit
{
    public class CustomerValidationsTests
    {
        // UTC-05: Valideer geldige e-mail
        [Fact]
        public void ValidEmail_ValidEmail_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";

            // Act
            bool result = CustomerValidation.ValidEmail(email);

            // Assert
            Assert.True(result);
        }

        // UTC-06: Valideer ongeldige e-mail (geen @)
        [Fact]
        public void ValidEmail_NoAtSymbol_ReturnsFalse()
        {
            // Arrange
            string email = "geen-email";

            // Act
            bool result = CustomerValidation.ValidEmail(email);

            // Assert
            Assert.False(result);
        }

        // UTC-07: Dubbele @
        [Fact]
        public void ValidEmail_DoubleAtSymbol_ReturnsFalse()
        {
            // Arrange
            string email = "test@@gmail.com";

            // Act
            bool result = CustomerValidation.ValidEmail(email);

            // Assert
            Assert.False(result);
        }


        // UTC-08: Telefoon input met letters
        [Fact]
        public void ValidatePhone_WithLetters_ReturnsFalse()
        {
            // Arrange
            string phone = "06ABC34284T";

            // Act
            bool result = CustomerValidation.ValidatePhone(phone);

            // Assert
            Assert.False(result);
        }


        // UTC-09: Valideer telefoonnummer
        [Fact]
        public void ValidatePhone_ValidPhone_ReturnsTrue()
        {
            // Arrange
            string phone = "0612345678";

            // Act
            bool result = CustomerValidation.ValidatePhone(phone);

            // Assert
            Assert.True(result);
        }

        // UTC-10: Valideer internationaal nummer
        [Fact]
        public void ValidatePhone_InternationalPhone_ReturnsTrue()
        {
            // Arrange
            string phone = "+31612345678";

            // Act
            bool result = CustomerValidation.ValidatePhone(phone);

            // Assert
            Assert.True(result);
        }




        // UTC-11: Valideer verplichte velden (alle velden aanwezig)
        [Fact]
        public void RequiredFieldsCheck_AllFieldsValid_ReturnsTrue()
        {
            // Arrange
            string firstName = "testVoornaam";
            string lastName = "testAchternaam";
            string email = "Test@gmail.com";
            string phone = "0612345678";

            // Act
            bool result = CustomerValidation.RequiredFieldsCheck(firstName, lastName, email, phone);

            // Assert
            Assert.True(result);
        }


        // UTC-12: Geen email en telefoonnummer
        [Fact]
        public void RequiredFieldsCheck_NoEmailAndPhone_ReturnsFalse()
        {
            // Arrange
            string firstName = "testVoornaam";
            string lastName = "testAchternaam";
            string email = "";
            string phone = "";

            // Act
            bool result = CustomerValidation.RequiredFieldsCheck(firstName, lastName, email, phone);

            // Assert
            Assert.False(result);
        }

        // UTC-13: Geen voor- en achternaam
        [Fact]
        public void RequiredFieldsCheck_ReturnsFalse()
        {
            // Arrange
            string firstName = "";
            string lastName = "";
            string email = "0612345678";
            string phone = "Test@gmail.com";

            // Act
            bool result = CustomerValidation.RequiredFieldsCheck(firstName, lastName, email, phone);

            // Assert
            Assert.False(result);
        }




    }
}