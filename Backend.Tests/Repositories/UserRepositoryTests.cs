using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Models;
using Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace Backend.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly Mock<DbSet<User>> _mockUserSet;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly UserRepository _repository;
        private readonly List<User> _users;

        public UserRepositoryTests()
        {
            // Initialize test data
            _users = new List<User>
            {
                new Student { Id = 1, Username = "student1", FirstName = "John", LastName = "Doe", Password = "password123" },
                new Teacher { Id = 2, Username = "teacher1", FirstName = "Jane", LastName = "Smith", Password = "password456" },
                new Admin { Id = 3, Username = "admin1", FirstName = "Admin", LastName = "User", Password = "adminpass" }
            };

            // Create mock DbSet for User
            _mockUserSet = new Mock<DbSet<User>>();

            // Setup IQueryable with an async query provider
            var asyncQueryProvider = new TestAsyncQueryProvider<User>(_users.AsQueryable());
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(asyncQueryProvider);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(_users.AsQueryable().Expression);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(_users.AsQueryable().ElementType);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => _users.GetEnumerator());

            // Setup IAsyncEnumerable for ToListAsync
            _mockUserSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(_users.GetEnumerator()));

            // Setup FindAsync for GetUserByIdAsync
            _mockUserSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => _users.FirstOrDefault(u => u.Id == (int)ids[0]));

            // Setup DbContextOptions for ApplicationDbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            // Initialize mock context with DbContextOptions
            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockContext.Setup(c => c.Set<User>()).Returns(_mockUserSet.Object);
            _mockContext.Setup(c => c.Add(It.IsAny<User>())).Callback<User>(user => _users.Add(user));
            _mockContext.Setup(c => c.Update(It.IsAny<User>())).Callback<User>(user =>
            {
                var existing = _users.FirstOrDefault(u => u.Id == user.Id);
                if (existing != null) { _users.Remove(existing); _users.Add(user); }
            });
            _mockContext.Setup(c => c.Remove(It.IsAny<User>())).Callback<User>(user => _users.Remove(user));
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Initialize repository
            _repository = new UserRepository(_mockContext.Object);
        }

        // Helper class for async enumeration
        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
            public T Current => _inner.Current;
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
            public ValueTask DisposeAsync() { _inner.Dispose(); return default; }
        }

        // Helper class to implement IAsyncQueryProvider
        private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            public TestAsyncQueryProvider(IQueryable<TEntity> inner)
            {
                _inner = inner.Provider;
            }

            public IQueryable CreateQuery(Expression expression) => _inner.CreateQuery(expression);
            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => _inner.CreateQuery<TElement>(expression);
            public object Execute(Expression expression) => _inner.Execute(expression);
            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            {
                // Execute synchronously and return the result directly
                return _inner.Execute<TResult>(expression);
            }
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            var result = await _repository.GetAllUsersAsync();
            Assert.Equal(_users.Count, result.Count);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
        {
            var result = await _repository.GetUserByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("student1", result.Username);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            var result = await _repository.GetUserByIdAsync(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAddUserAndSaveChanges()
        {
            var newUser = new Student
            {
                Username = "newstudent",
                FirstName = "New",
                LastName = "Student",
                Password = "newpass"
            };
            var result = await _repository.CreateUserAsync(newUser);
            Assert.NotNull(result);
            Assert.Equal("newstudent", result.Username);
            _mockContext.Verify(c => c.Add(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUserAndSaveChanges()
        {
            var userToUpdate = _users.First();
            userToUpdate.FirstName = "UpdatedName";
            await _repository.UpdateUserAsync(userToUpdate);
            _mockContext.Verify(c => c.Update(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WithValidId_ShouldRemoveUserAndSaveChanges()
        {
            var userToDelete = _users.First();
            await _repository.DeleteUserAsync(userToDelete.Id);
            _mockContext.Verify(c => c.Remove(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WithInvalidId_ShouldNotCallRemove()
        {
            await _repository.DeleteUserAsync(999);
            _mockContext.Verify(c => c.Remove(It.IsAny<User>()), Times.Never);
        }
    }
}