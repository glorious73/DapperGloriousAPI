using Domain.Base;
using Domain.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.Contracts.Account;

namespace Infrastructure.UserRepository
{
    public class UserRepository : IUserRepository
    {
        public ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// get a list
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderByDate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationUser>> Get(UserQueryFilter filter)
        {
            using var connection = _context.CreateConnection();
            var where = BuildQuery(filter);
            var sql = "SELECT * FROM ApplicationUsers " 
                      + where
                      + "ORDER BY CreatedAt DESC "
                      + "LIMIT @Limit OFFSET @Offset";
            return await connection.QueryAsync<ApplicationUser>(sql, filter);
        }

        public async Task<ApplicationUser> GetById(int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM ApplicationUsers WHERE Id = @id";
            return await connection.QuerySingleOrDefaultAsync<ApplicationUser>(sql, new { id });
        }

        /// <summary>
        /// count a list
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>count of items in the query</returns>
        public async Task<int> Count(UserQueryFilter filter)
        {
            using var connection = _context.CreateConnection();
            var where = BuildQuery(filter);
            var sql = "SELECT COUNT(*) FROM ApplicationUsers " + where;
            return await connection.QueryFirstAsync<int>(sql, filter);
        }

        public async Task Insert(ApplicationUser entity)
        {
            using var connection = _context.CreateConnection();
            var sql = "INSERT INTO ApplicationUsers (Username, FirstName, LastName, EmailAddress, Role, PasswordHash, PasswordSalt, NumberOfLogins, IsEnabled) " +
                      "VALUES (@Username, @FirstName, @LastName, @EmailAddress, @Role, @PasswordHash, @PasswordSalt, @NumberOfLogins, @IsEnabled)";
            await connection.ExecuteAsync(sql, entity);
        }

        public async Task Delete(int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM ApplicationUsers " +
                      "WHERE Id = @id";
            await connection.ExecuteAsync(sql, new { id });
        }

        public async Task Update(ApplicationUser entityToUpdate)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE ApplicationUsers  SET " +
                      "Username = @Username, " +
                      "FirstName = @FirstName, " +
                      "LastName = @LastName, " +
                      "EmailAddress = @EmailAddress, " +
                      "Role = @Role, " +
                      "PasswordHash = @PasswordHash, " +
                      "PasswordSalt = @PasswordSalt, " +
                      "IsEnabled = @IsEnabled " +
                      "WHERE Id = @Id";
            await connection.ExecuteAsync(sql, entityToUpdate);
        }

        public async Task<bool> Any(string filter = null)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT EXISTS FROM ApplicationUsers " 
                      + ((filter ==null) ? "" : filter);
            return await connection.QueryFirstAsync<bool>(sql);
        }

        private string BuildQuery(UserQueryFilter filter)
        {
            string where;
            if (filter.Role == 0 && filter.CreatedStart == null && filter.CreatedEnd == null && filter.EmailAddress == null)
                where = "";
            else
            {
                // 1. build separate queries
                var queries = new string?[4];
                if (filter.EmailAddress != null)
                    queries[0] = "EmailAddress LIKE '%' || @EmailAddress || '%'";
                if (filter.CreatedStart != null)
                    queries[1] = "CreatedAt >= @CreatedStart";
                if (filter.CreatedEnd != null)
                    queries[2] = "CreatedAt <= @CreatedEnd";
                if (filter.Role != 0)
                    queries[3] = "Role = @Role";
                // 2. combine queries
                queries = queries.Where(q => q != null).ToArray();
                var queriesString = "";
                for (int i = 0; i < queries.Length; i++)
                    queriesString   += (i > 0) ? $"AND {queries[i]} " : $"{queries[i]} ";
                // 3. Add WHERE
                where = "WHERE " + queriesString;
            }
            return where;
        }
    }
}
