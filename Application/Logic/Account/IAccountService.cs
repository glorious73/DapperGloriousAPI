using Application.DTO.Account;
using Application.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts.Account;
using Infrastructure.UserRepository;

namespace Application.Logic.Account
{
    public interface IAccountService
    {
        Task<ApplicationUser> Create(UserDTO RequestUser, string Password);
        Task<ApplicationUser> Edit(int accountId, UserEditDTO userToEdit);
        Task<ApplicationUser> GetById(int Id);
        Task<IEnumerable<ApplicationUser>> GetAll(UserQueryFilter queryFilter);
        Task<int> CountAll(UserQueryFilter queryFilter);
        Task<bool> Delete(int accountId);
        HashPasswordDTO HashPassword(string password, byte[] salt = null);
    }
}
