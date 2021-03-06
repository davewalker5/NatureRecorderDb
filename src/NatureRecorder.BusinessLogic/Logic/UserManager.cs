﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Entities.Interfaces;

namespace NatureRecorder.BusinessLogic.Logic
{
    internal class UserManager : IUserManager
    {
        private readonly Lazy<PasswordHasher<string>> _hasher;
        private readonly NatureRecorderDbContext _context;

        public UserManager(NatureRecorderDbContext context)
        {
            _hasher = new Lazy<PasswordHasher<string>>(() => new PasswordHasher<string>());
            _context = context;
        }

        /// <summary>
        /// Return the user with the specified Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User GetUser(int userId)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            ThrowIfUserNotFound(user, userId);
            return user;
        }

        /// <summary>
        /// Return the user with the specified Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(int userId)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            ThrowIfUserNotFound(user, userId);
            return user;
        }

        /// <summary>
        /// Return the user with the specified Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User GetUser(string userName)
        {
            User user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            ThrowIfUserNotFound(user, userName);
            return user;
        }

        /// <summary>
        /// Return the user with the specified Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(string userName)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            ThrowIfUserNotFound(user, userName);
            return user;
        }

        /// <summary>
        /// Get all the current user details
        /// </summary>
        public IEnumerable<User> GetUsers() =>
            _context.Users;

        /// <summary>
        /// Get all the current user details
        /// </summary>
        public IAsyncEnumerable<User> GetUsersAsync() =>
            _context.Users.AsAsyncEnumerable();

        /// <summary>
        /// Add a new user, given their details
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User AddUser(string userName, string password)
        {
            User user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            ThrowIfUserFound(user, userName);

            user = new User
            {
                UserName = userName,
                Password = _hasher.Value.HashPassword(userName, password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        /// <summary>
        /// Add a new user, given their details
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> AddUserAsync(string userName, string password)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            ThrowIfUserFound(user, userName);

            user = new User
            {
                UserName = userName,
                Password = _hasher.Value.HashPassword(userName, password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Authenticate the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string userName, string password)
        {
            User user = GetUser(userName);
            PasswordVerificationResult result = _hasher.Value.VerifyHashedPassword(userName, user.Password, password);
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _hasher.Value.HashPassword(userName, password);
                _context.SaveChanges();
            }
            return result != PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// Authenticate the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> AuthenticateAsync(string userName, string password)
        {
            User user = await GetUserAsync(userName);
            PasswordVerificationResult result = _hasher.Value.VerifyHashedPassword(userName, user.Password, password);
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _hasher.Value.HashPassword(userName, password);
                await _context.SaveChangesAsync();
            }
            return result != PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// Set the password for the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void SetPassword(string userName, string password)
        {
            User user = GetUser(userName);
            user.Password = _hasher.Value.HashPassword(userName, password);
            _context.SaveChanges();
        }

        /// <summary>
        /// Set the password for the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public async Task SetPasswordAsync(string userName, string password)
        {
            User user = await GetUserAsync(userName);
            user.Password = _hasher.Value.HashPassword(userName, password);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete the specified user
        /// </summary>
        /// <param name="userName"></param>
        public void DeleteUser(string userName)
        {
            User user = GetUser(userName);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete the specified user
        /// </summary>
        /// <param name="userName"></param>
        public async Task DeleteUserAsync(string userName)
        {
            User user = await GetUserAsync(userName);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Throw an exception if a user doesn't exist
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfUserNotFound(User user, object userId)
        {
            if (user == null)
            {
                string message = $"User {userId} not found";
                throw new UserNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an exception if a user already exists
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfUserFound(User user, object userId)
        {
            if (user != null)
            {
                throw new UserExistsException($"User {userId} already exists");
            }
        }
    }
}
