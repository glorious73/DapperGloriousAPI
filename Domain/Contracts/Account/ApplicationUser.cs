using Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Contracts.Account
{
    public class ApplicationUser : Base.Entity
    {
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
        public string? Token { get; set; } // Json Web Token
        public string? PasswordResetToken { get; set; }
        public string? PasswordOTP { get; set; }
        public DateTime LastLogin { get; set; }
        public int NumberOfLogins { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
