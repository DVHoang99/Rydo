using Rydo.Application.Common.Enums;

namespace Rydo.Domain.Entities;

public class User
{
    public string PhoneNumber { get; set; }
    public string PassWord { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserStatus Status { get; set; }
    public UserType Type { get; set; }
}