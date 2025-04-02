using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Controllers;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Backend.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UsersController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOkResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { Id = 1, Username = "user1", FirstName = "First1", LastName = "Last1", Role = "Student" },
                new UserDto { Id = 2, Username = "user2", FirstName = "First2", LastName = "Last2", Role = "Teacher" }
            };
            
            _mockUserService.Setup(service => service.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUsers = Assert.IsType<List<UserDto>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count);
        }

        [Fact]
        public async Task GetUser_WithValidId_ShouldReturnOkResult_WithUser()
        {
            // Arrange
            var user = new UserDto 
            { 
                Id = 1, 
                Username = "user1", 
                FirstName = "First1", 
                LastName = "Last1", 
                Role = "Student" 
            };
            
            _mockUserService.Setup(service => service.GetUserByIdAsync(1))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(1, returnedUser.Id);
            Assert.Equal("user1", returnedUser.Username);
        }

        [Fact]
        public async Task GetUser_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockUserService.Setup(service => service.GetUserByIdAsync(999))
                .ReturnsAsync((UserDto)null);

            // Act
            var result = await _controller.GetUser(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateUser_WithValidData_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Username = "newuser",
                FirstName = "New",
                LastName = "User",
                Password = "password",
                Role = "Student"
            };
            
            var createdUser = new UserDto 
            { 
                Id = 3, 
                Username = "newuser", 
                FirstName = "New", 
                LastName = "User", 
                Role = "Student" 
            };
            
            _mockUserService.Setup(service => service.CreateUserAsync(createUserDto))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.CreateUser(createUserDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedUser = Assert.IsType<UserDto>(createdAtActionResult.Value);
            Assert.Equal(3, returnedUser.Id);
            Assert.Equal("newuser", returnedUser.Username);
        }

        [Fact]
        public async Task CreateUser_WithInvalidData_ShouldReturnBadRequest()
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
            
            _mockUserService.Setup(service => service.CreateUserAsync(createUserDto))
                .ThrowsAsync(new InvalidOperationException("Username already exists"));

            // Act
            var result = await _controller.CreateUser(createUserDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Username already exists", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateUser_WithValidData_ShouldReturnNoContent()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto
            {
                FirstName = "Updated",
                LastName = "User",
                Password = "newpassword"
            };
            
            _mockUserService.Setup(service => service.UpdateUserAsync(1, updateUserDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateUser(1, updateUserDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUser_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto
            {
                FirstName = "Updated",
                LastName = "User",
                Password = "newpassword"
            };
            
            _mockUserService.Setup(service => service.UpdateUserAsync(999, updateUserDto))
                .ThrowsAsync(new InvalidOperationException("User not found"));

            // Act
            var result = await _controller.UpdateUser(999, updateUserDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnNoContent()
        {
            // Arrange
            _mockUserService.Setup(service => service.DeleteUserAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }

    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _mockAdminService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller = new AdminController(_mockAdminService.Object);
        }

        [Fact]
        public async Task AddTeacher_ShouldReturnCreatedAtAction_WithTeacher()
        {
            // Arrange
            var teacherDto = new CreateUserDto
            {
                Username = "newteacher",
                FirstName = "New",
                LastName = "Teacher",
                Password = "password",
                Role = "Teacher"
            };
            
            var createdTeacher = new UserDto 
            { 
                Id = 1, 
                Username = "newteacher", 
                FirstName = "New", 
                LastName = "Teacher", 
                Role = "Teacher" 
            };
            
            _mockAdminService.Setup(service => service.AddTeacherAsync(teacherDto))
                .ReturnsAsync(createdTeacher);

            // Act
            var result = await _controller.AddTeacher(teacherDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedTeacher = Assert.IsType<UserDto>(createdAtActionResult.Value);
            Assert.Equal(1, returnedTeacher.Id);
            Assert.Equal("newteacher", returnedTeacher.Username);
        }

        [Fact]
        public async Task ViewTeachers_ShouldReturnOkResult_WithListOfTeachers()
        {
            // Arrange
            var teachers = new List<UserDto>
            {
                new UserDto { Id = 1, Username = "teacher1", FirstName = "First1", LastName = "Last1", Role = "Teacher" },
                new UserDto { Id = 2, Username = "teacher2", FirstName = "First2", LastName = "Last2", Role = "Teacher" }
            };
            
            _mockAdminService.Setup(service => service.ViewTeachersAsync())
                .ReturnsAsync(teachers);

            // Act
            var result = await _controller.ViewTeachers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTeachers = Assert.IsType<List<UserDto>>(okResult.Value);
            Assert.Equal(2, returnedTeachers.Count);
        }

        [Fact]
        public async Task AddCourse_ShouldReturnCreatedAtAction_WithCourse()
        {
            // Arrange
            var courseDto = new CreateCourseDto
            {
                CourseName = "Math 101",
                TeacherId = 1
            };
            
            var createdCourse = new CourseDto 
            { 
                Id = 1, 
                CourseName = "Math 101", 
                TeacherId = 1 
            };
            
            _mockAdminService.Setup(service => service.AddCourseAsync(courseDto))
                .ReturnsAsync(createdCourse);

            // Act
            var result = await _controller.AddCourse(courseDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedCourse = Assert.IsType<CourseDto>(createdAtActionResult.Value);
            Assert.Equal(1, returnedCourse.Id);
            Assert.Equal("Math 101", returnedCourse.CourseName);
        }
    }
}