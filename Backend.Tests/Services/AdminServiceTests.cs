using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Backend.Repositories.Interfaces;
using Backend.Services;
using Moq;
using Xunit;

namespace Backend.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly Mock<IGradeRepository> _mockGradeRepository;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCourseRepository = new Mock<ICourseRepository>();
            _mockGradeRepository = new Mock<IGradeRepository>();
            
            _adminService = new AdminService(
                _mockUserRepository.Object,
                _mockCourseRepository.Object,
                _mockGradeRepository.Object);
        }

        [Fact]
        public async Task AddTeacherAsync_WithValidData_ShouldCreateTeacher()
        {
            // Arrange
            var teacherDto = new CreateUserDto
            {
                Username = "newteacher",
                FirstName = "New",
                LastName = "Teacher",
                Password = "password123",
                Role = "Teacher"
            };
            
            Teacher createdTeacher = null;
            
            _mockUserRepository.Setup(repo => repo.UsernameExistsAsync("newteacher"))
                .ReturnsAsync(false);
                
            _mockUserRepository.Setup(repo => repo.CreateUserAsync(It.IsAny<Teacher>()))
                .Callback<User>(u => createdTeacher = u as Teacher)
                .ReturnsAsync((User u) => 
                {
                    u.Id = 1;
                    return u;
                });

            // Act
            var result = await _adminService.AddTeacherAsync(teacherDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("newteacher", result.Username);
            Assert.Equal("Teacher", result.Role);
            
            _mockUserRepository.Verify(repo => repo.CreateUserAsync(It.IsAny<Teacher>()), Times.Once);
        }

        [Fact]
        public async Task AddTeacherAsync_WithInvalidRole_ShouldThrowException()
        {
            // Arrange
            var teacherDto = new CreateUserDto
            {
                Username = "newteacher",
                FirstName = "New",
                LastName = "Teacher",
                Password = "password123",
                Role = "Student"  // Invalid role for a teacher
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _adminService.AddTeacherAsync(teacherDto));
                
            _mockUserRepository.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task DeleteTeacherAsync_WithValidId_ShouldDeleteTeacher()
        {
            // Arrange
            var teacher = new Teacher 
            { 
                Id = 1, 
                Username = "teacher1", 
                FirstName = "Jane", 
                LastName = "Smith"
            };
            
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1))
                .ReturnsAsync(teacher);
                
            _mockUserRepository.Setup(repo => repo.DeleteUserAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            await _adminService.DeleteTeacherAsync(1);

            // Assert
            _mockUserRepository.Verify(repo => repo.DeleteUserAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteTeacherAsync_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(999))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _adminService.DeleteTeacherAsync(999));
                
            _mockUserRepository.Verify(repo => repo.DeleteUserAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ViewTeachersAsync_ShouldReturnOnlyTeachers()
        {
            // Arrange
            var users = new List<User>
            {
                new Student { Id = 1, Username = "student1", FirstName = "John", LastName = "Doe" },
                new Teacher { Id = 2, Username = "teacher1", FirstName = "Jane", LastName = "Smith" },
                new Teacher { Id = 3, Username = "teacher2", FirstName = "Bob", LastName = "Johnson" }
            };
            
            _mockUserRepository.Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _adminService.ViewTeachersAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("teacher1", result[0].Username);
            Assert.Equal("teacher2", result[1].Username);
            Assert.All(result, item => Assert.Equal("Teacher", item.Role));
        }

        [Fact]
        public async Task AddCourseAsync_WithValidData_ShouldCreateCourse()
        {
            // Arrange
            var teacher = new Teacher { Id = 1 };
            var courseDto = new CreateCourseDto
            {
                Title = "Math 101",
                TeacherId = 1
            };
            
            Course createdCourse = null;
            
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1))
                .ReturnsAsync(teacher);
                
            _mockCourseRepository.Setup(repo => repo.CreateCourseAsync(It.IsAny<Course>()))
                .Callback<Course>(c => createdCourse = c)
                .ReturnsAsync((Course c) => 
                {
                    c.Id = 1;
                    return c;
                });

            // Act
            var result = await _adminService.AddCourseAsync(courseDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Math 101", result.Title);
            Assert.Equal(1, result.TeacherId);
            
            _mockCourseRepository.Verify(repo => repo.CreateCourseAsync(It.IsAny<Course>()), Times.Once);
        }

        [Fact]
        public async Task AddCourseAsync_WithInvalidTeacher_ShouldThrowException()
        {
            // Arrange
            var courseDto = new CreateCourseDto
            {
                Title = "Math 101",
                TeacherId = 999 // Non-existent teacher
            };
            
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(999))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _adminService.AddCourseAsync(courseDto));
                
            _mockCourseRepository.Verify(repo => repo.CreateCourseAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task AddStudentAsync_WithValidData_ShouldCreateStudent()
        {
            // Arrange
            var studentDto = new CreateUserDto
            {
                Username = "newstudent",
                FirstName = "New",
                LastName = "Student",
                Password = "password456",
                Role = "Student"
            };
            
            Student createdStudent = null;
            
            _mockUserRepository.Setup(repo => repo.UsernameExistsAsync("newstudent"))
                .ReturnsAsync(false);
                
            _mockUserRepository.Setup(repo => repo.CreateUserAsync(It.IsAny<Student>()))
                .Callback<User>(u => createdStudent = u as Student)
                .ReturnsAsync((User u) => 
                {
                    u.Id = 2;
                    return u;
                });

            // Act
            var result = await _adminService.AddStudentAsync(studentDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("newstudent", result.Username);
            Assert.Equal("Student", result.Role);
            
            _mockUserRepository.Verify(repo => repo.CreateUserAsync(It.IsAny<Student>()), Times.Once);
        }

        [Fact]
        public async Task AddGradeAsync_WithValidData_ShouldCreateGrade()
        {
            // Arrange
            var student = new Student { Id = 1 };
            var course = new Course { Id = 1 };
            var gradeDto = new CreateGradeDto
            {
                StudentId = 1,
                CourseId = 1,
                Value = 85
            };
            
            Grade createdGrade = null;
            
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1))
                .ReturnsAsync(student);
                
            _mockCourseRepository.Setup(repo => repo.GetCourseByIdAsync(1))
                .ReturnsAsync(course);
                
            _mockGradeRepository.Setup(repo => repo.CreateGradeAsync(It.IsAny<Grade>()))
                .Callback<Grade>(g => createdGrade = g)
                .ReturnsAsync((Grade g) => 
                {
                    g.Id = 1;
                    return g;
                });

            // Act
            var result = await _adminService.AddGradeAsync(gradeDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(1, result.StudentId);
            Assert.Equal(1, result.CourseId);
            Assert.Equal(85, result.Value);
            
            _mockGradeRepository.Verify(repo => repo.CreateGradeAsync(It.IsAny<Grade>()), Times.Once);
        }
    }
}