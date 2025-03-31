using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<UserDto> CreateUserAsync(CreateUserDto userDto);
        Task UpdateUserAsync(int id, UpdateUserDto userDto);
        Task DeleteUserAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(MapUserToDto(user));
            }

            return userDtos;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return null;

            return MapUserToDto(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
                return null;

            return MapUserToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto userDto)
        {
            // Check if username already exists
            bool usernameExists = await _userRepository.UsernameExistsAsync(userDto.Username);
            if (usernameExists)
                throw new InvalidOperationException("Username already exists");

            // Create appropriate user type based on role
            User user;
            if (userDto.Role.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                user = new Student();
            }
            else if (userDto.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
            {
                user = new Teacher();
            }
            else
            {
                throw new InvalidOperationException("Invalid user role");
            }

            // Set user properties
            user.Username = userDto.Username;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Password = userDto.Password; // In a real app, you would hash this password

            // Save to database
            user = await _userRepository.CreateUserAsync(user);

            // Return DTO
            return MapUserToDto(user);
        }

        public async Task UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Update only properties that are provided
            if (!string.IsNullOrEmpty(userDto.FirstName))
                user.FirstName = userDto.FirstName;

            if (!string.IsNullOrEmpty(userDto.LastName))
                user.LastName = userDto.LastName;

            if (!string.IsNullOrEmpty(userDto.Password))
                user.Password = userDto.Password; // In a real app, you would hash this password

            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        private UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.UserRole.ToString()
            };
        }
    }
}