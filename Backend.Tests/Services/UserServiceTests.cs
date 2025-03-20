using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Backend.Services;
using Moq;
using Xunit;

namespace Backend.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new Student { Id = 1, Username = "student1", FirstName = "John", LastName = "Doe", Password = "password123" },
                new Teacher { Id = 2, Username = "teacher1", FirstName = "Jane", LastName = "Smith", Password = "password456" }
            };

            _mockUserRepository.Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("student1", result[0].Username);
            Assert.Equal("teacher1", result[1].Username);
            Assert.Equal("Student", result[0].Role);
            Assert.Equal("Teacher", result[1].Role);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var user = new Student
            {
                Id = 1,
                Username = "student1",
                FirstName = "John",
                LastName = "Doe",
                Password = "password123"
            };

            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("student1", result.Username);
            Assert.Equal("Student", result.Role);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(999))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WithValidUsername_ShouldReturnUser()
        {
            // Arrange
            var user = new Teacher
            {
                Id = 2,
                Username = "teacher1",
                FirstName = "Jane",
                LastName = "Smith",
                Password = "password456"
            };

            _mockUserRepository.Setup(repo => repo.GetUserByUsernameAsync("teacher1"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByUsernameAsync("teacher1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("teacher1", result.Username);
            Assert.Equal("Teacher", result.Role);
        }

        [Fact]
        public async Task CreateUserAsync_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Username = "newstudent",
                FirstName = "New",
                LastName = "Student",
                Password = "password789",
                Role = "Student"
            };

            Student createdStudent = null;

            _mockUserRepository.Setup(repo => repo.UsernameExistsAsync("newstudent"))
                .ReturnsAsync(false);

            _mockUserRepository.Setup(repo => repo.CreateUserAsync(It.IsAny<Student>()))
                .Callback<User>(u => createdStudent = u as Student)
                .ReturnsAsync((User u) =>
                {
                    u.Id = 3;
                    return u;
                });

            // Act
            var result = await _userService.CreateUserAsync(createUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Equal("newstudent", result.Username);
            Assert.Equal("Student", result.Role);

            _mockUserRepository.Verify(repo => repo.CreateUserAsync(It.IsAny<Student>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_WithDuplicateUsername_ShouldThrowException()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Username = "existinguser",
                FirstName = "Existing",
                LastName = "User",
                Password = "password",
                Role = "Student"
            };

            _mockUserRepository.Setup(repo => repo.UsernameExistsAsync("existinguser"))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.CreateUserAsync(createUserDto));

            _mockUserRepository.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidData_ShouldUpdateUser()
        {
            // Arrange
            var user = new Student
            {
                Id = 1,
                Username = "student1",
                FirstName = "John",
                LastName = "Doe",
                Password = "password123"
            };

            var updateUserDto = new UpdateUserDto
            {
                FirstName = "Johnny",
                LastName = "Updated",
                Password = "newpassword"
            };

            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.UpdateUserAsync(1, updateUserDto);

            // Assert
            _mockUserRepository.Verify(repo => repo.UpdateUserAsync(It.Is<User>(u =>
                u.FirstName == "Johnny" &&
                u.LastName == "Updated" &&
                u.Password == "newpassword")), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldCallRepository()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.DeleteUserAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteUserAsync(1);

            // Assert
            _mockUserRepository.Verify(repo => repo.DeleteUserAsync(1), Times.Once);
        }
    }
}