using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Account
{
    public class UserEditDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Username { get; set; }
        public string EmailAddress { get; set; }
        public Domain.Enums.Role Role { get; set; } = 0;
        public string IsEnabled { get; set; }
    }
}
