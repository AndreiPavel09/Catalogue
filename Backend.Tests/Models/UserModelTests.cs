using Backend.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;
using System.Collections.Generic;

namespace Backend.Tests.Models
{
    public class UserModelTests
    {
        [Fact]
        public void Student_Constructor_SetsRoleToStudent()
        {
            // Arrange & Act
            var student = new Student();

            // Assert
            Assert.Equal(UserRole.Student, student.UserRole);
        }

        [Fact]
        public void Teacher_Constructor_SetsRoleToTeacher()
        {
            // Arrange & Act
            var teacher = new Teacher();

            // Assert
            Assert.Equal(UserRole.Teacher, teacher.UserRole);
        }

        [Fact]
        public void Admin_Constructor_SetsRoleToAdmin()
        {
            // Arrange & Act
            var admin = new Admin();

            // Assert
            Assert.Equal(UserRole.Admin, admin.UserRole);
        }

        [Fact]
        public void FullName_ReturnsCorrectConcatenation()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var fullName = student.FullName;

            // Assert
            Assert.Equal("John Doe", fullName);
        }

        [Fact]
        public void User_ValidateRequiredFields()
        {
            // Arrange
            var student = new Student();
            var context = new ValidationContext(student);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(student, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Equal(4, validationResults.Count); // Username, FirstName, LastName, Password are required
        }

        [Fact]
        public void User_ValidateStringLengthAttributes()
        {
            // Arrange
            var student = new Student
            {
                Username = new string('a', 51), // Exceeds StringLength(50)
                FirstName = new string('b', 101), // Exceeds StringLength(100)
                LastName = new string('c', 101), // Exceeds StringLength(100)
                Password = new string('d', 101) // Exceeds StringLength(100)
            };
            var context = new ValidationContext(student);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(student, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Equal(4, validationResults.Count); // All fields exceed max length
        }

        [Fact]
        public void User_ValidModel_ShouldPassValidation()
        {
            // Arrange
            var student = new Student
            {
                Username = "validusername",
                FirstName = "John",
                LastName = "Doe",
                Password = "validpassword"
            };
            var context = new ValidationContext(student);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(student, context, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}