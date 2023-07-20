using Application.DTO.Account;
using Application.DTO.Auth;
using Domain.Contracts;
using Domain.Enums;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.Contracts.Account;
using Infrastructure.UserRepository;
using Utility;

namespace Application.Logic.Account
{
    public class AccountService : IAccountService
    {
        private IUserRepository _repository;
        private IConfiguration _config { get; }
        private IExpressionUtility _expressionUtility;

        public AccountService(IUserRepository repository, IConfiguration config, IExpressionUtility expressionUtility)
        {
            _repository = repository;
            _config = config;
            _expressionUtility = expressionUtility;
        }

        public async Task<ApplicationUser> Create(UserDTO userDTO, string Password)
        {
            bool isValidUser = IsValidUser(userDTO);
            // Create User
            var user = new ApplicationUser()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                EmailAddress = userDTO.EmailAddress.ToLower(),
                Username = userDTO.Username.ToLower(),
                Role = userDTO.Role,
                IsEnabled = true
            };
            // Hash password
            HashPasswordDTO hashResult = HashPassword(Password);
            user.PasswordHash = hashResult.Hash;
            user.PasswordSalt = Convert.ToBase64String(hashResult.Salt);
            // Create user
            await _repository.Insert(user);
            // Done
            return user;
        }

        public async Task<ApplicationUser> Edit(int accountId, UserEditDTO userDTO)
        {
            // Find
            ApplicationUser user = await GetById(accountId);
            // Validate
            bool result = IsValidEdit(userDTO);
            // Edit
            user.FirstName = userDTO.FirstName;
            user.LastName = userDTO.LastName;
            user.Username = userDTO.Username != null ? userDTO.Username.ToLower() : user.Username;
            user.EmailAddress = userDTO.EmailAddress;
            user.Role = userDTO.Role;
            user.IsEnabled = userDTO.IsEnabled.ToLower() == "true";
            user.UpdatedAt = DateTime.UtcNow;
            // Update
            await _repository.Update(user);
            // Done
            return user;
        }

        public async Task<bool> Delete(int accountId)
        {
            var account = await GetById(accountId);
            bool isValid = isValidDelete(account);
            await _repository.Delete(accountId);
            // Done
            return true;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAll(UserQueryFilter queryFilter)
        {
            //Expression<Func<ApplicationUser, bool>>? filter = FilterAll(filterUserDTO);
            // Get users
            var users = await _repository.Get(queryFilter);
            // return users
            return users;
        }

        public async Task<int> CountAll(UserQueryFilter queryFilter)
        {
            //Expression<Func<ApplicationUser, bool>>? filter = FilterAll(filterUserDTO);
            return await _repository.Count(queryFilter);
        }

        public async Task<ApplicationUser> GetById(int Id)
        {
            var user = await _repository.GetById(Id);
            if (user == null)
                throw new InvalidOperationException("User was not found.");
            // Done
            return user;
        }

        public HashPasswordDTO HashPassword(string password, byte[] salt = null)
        {
            // generate a 128-bit salt using a secure PRNG
            if (salt == null)
            {
                salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
            }
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            // Done
            return new HashPasswordDTO() { Hash = hashed, Salt = salt };
        }

        private bool IsValidUser(UserDTO userDTO)
        {
            // Validate Email
            try
            {
                MailAddress mailAddress = new MailAddress(userDTO.EmailAddress);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Please enter a valid email address.");
            }
            // Validate Username
            if (!string.IsNullOrEmpty(userDTO.Username) && userDTO.Username.Contains(" "))
                throw new InvalidOperationException("Username cannot contain spaces.");
            return true;
        }

        private bool IsValidEdit(UserEditDTO userDTO)
        {
            // Validate Email
            try
            {
                MailAddress mailAddress = new MailAddress(userDTO.EmailAddress);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Please enter a valid email.");
            }
            // Validate Username
            if (!string.IsNullOrEmpty(userDTO.Username) && userDTO.Username.Contains(" "))
                throw new InvalidOperationException("Username cannot contain spaces.");
            return true;
        }

        private bool isValidDelete(ApplicationUser user)
        {
            // Admin
            if (user.Username == "admin")
                throw new InvalidOperationException("Cannot delete default account.");
            return true;
        }
    }
}
