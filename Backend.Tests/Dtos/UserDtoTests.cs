using Backend.DTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;
using System.Collections.Generic;

namespace Backend.Tests.DTOs
{
    public class UserDtoTests
    {
        [Fact]
        public void CreateUserDto_ValidateRequiredFields()
        {
            // Arrange
            var dto = new CreateUserDto();
            var context = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Equal(5, validationResults.Count); // Username, FirstName, LastName, Password, Role are required
        }

        [Fact]
        public void CreateUserDto_ValidateStringLengthAttributes()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Username = new string('a', 51), // Exceeds StringLength(50)
                FirstName = new string('b', 101), // Exceeds StringLength(100)
                LastName = new string('c', 101), // Exceeds StringLength(100)
                Password = new string('d', 101), // Exceeds StringLength(100)
                Role = "Student"
            };
            var context = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Equal(4, validationResults.Count); // All fields except Role exceed max length
        }

        [Fact]
        public void CreateUserDto_ValidModel_ShouldPassValidation()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Username = "validusername",
                FirstName = "John",
                LastName = "Doe",
                Password = "validpassword",
                Role = "Student"
            };
            var context = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, context, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void UpdateUserDto_ValidateStringLengthAttributes()
        {
            // Arrange
            var dto = new UpdateUserDto
            {
                FirstName = new string('b', 101), // Exceeds StringLength(100)
                LastName = new string('c', 101), // Exceeds StringLength(100)
                Password = new string('d', 101) // Exceeds StringLength(100)
            };
            var context = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Equal(3, validationResults.Count); // All fields exceed max length
        }

        [Fact]
        public void UpdateUserDto_ValidModel_ShouldPassValidation()
        {
            // Arrange
            var dto = new UpdateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Password = "validpassword"
            };
            var context = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, context, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void UserDto_FullNameProperty_ReturnsCorrectConcatenation()
        {
            // Arrange
            var dto = new UserDto
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var fullName = dto.FullName;

            // Assert
            Assert.Equal("John Doe", fullName);
        }
    }
}