namespace Rydo.Application.Interfaces.Password;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hashed);
}

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password, string hashed) => BCrypt.Net.BCrypt.Verify(password, hashed);
}