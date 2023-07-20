using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Domain.Database
{
    public class ApplicationDbContext
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
        
        public async Task Init()
        {
            // create database tables if they don't exist
            using var connection = CreateConnection();
            await _initUsers();

            async Task _initUsers()
            {
                var sql = "CREATE TABLE IF NOT EXISTS  ApplicationUsers " +
                          "( Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                          "CreatedAt datetime default current_timestamp, " +
                          "UpdatedAt datetime default current_timestamp," +
                          "Username TEXT, " +
                          "FirstName TEXT, " +
                          "LastName TEXT, " +
                          "EmailAddress TEXT, " +
                          "Role INTEGER, " +
                          "PasswordHash TEXT, " +
                          "PasswordSalt TEXT, " +
                          "PasswordResetToken TEXT, " +
                          "PasswordOTP TEXT, " +
                          "Token TEXT, " +
                          "LastLogin datetime default current_timestamp, " +
                          "NumberOfLogins INTEGER, " +
                          "IsEnabled BOOLEAN);";
                int result = await connection.ExecuteAsync(sql);
                return;
            }
        }
    }
}
