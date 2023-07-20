using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts.Account;
using Infrastructure.UserRepository;

namespace Infrastructure.UserRepository
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> Get(UserQueryFilter filter);
        Task<int> Count(UserQueryFilter filter);

        Task<ApplicationUser> GetById(int id);
        Task Insert(ApplicationUser entity);

        Task Delete(int id);

        Task Update(ApplicationUser entityToUpdate);
        Task<bool> Any(string filter);
    }
}
