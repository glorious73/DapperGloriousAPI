using System.Data;
using Domain.Enums;

namespace Infrastructure.UserRepository;

public class UserQueryFilter
{
    public Role Role { get; set; } = 0;
    public DateTime? CreatedStart { get; set; }
    public DateTime? CreatedEnd { get; set; }
    public string? EmailAddress { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
}