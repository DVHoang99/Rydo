using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rydo.Application.Common.Interfaces;
using Rydo.Application.Interfaces.Password;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Rydo.Application.Accounts.Commands;

public class LoginCommand(string phoneNumber, string password) : IRequest<string>
{
    public string PhoneNumber { get; } = phoneNumber;
    public string Password { get; } = password;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IApplicationDbContext _db;
    private readonly IConfiguration _config;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IApplicationDbContext db, IConfiguration config, IPasswordHasher passwordHasher)
    {
        _db = db;
        _config = config;
        _passwordHasher = passwordHasher;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber, cancellationToken);
        if (user == null) throw new Exception("Invalid credentials");

        if (!_passwordHasher.Verify(request.Password, user.PassWord))
            throw new Exception("Invalid credentials");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.PhoneNumber),
            new Claim("UserId", user.PhoneNumber),
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
