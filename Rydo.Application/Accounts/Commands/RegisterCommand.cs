using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rydo.Application.Common.Enums;
using Rydo.Application.Common.Helpers;
using Rydo.Application.Common.Interfaces;
using Rydo.Application.Interfaces.Password;
using Rydo.Domain.Entities;

namespace Rydo.Application.Accounts.Commands;

public class RegisterCommand(string phoneNumber, string password, string email, string firstName, string lastName) : IRequest<bool>
{
    public string PhoneNumber { get; } = phoneNumber;
    public string Password { get; } = password;
    public string Email { get; } = email;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
}

public class RegisterCommandHandler(IApplicationDbContext db, IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCommand, bool>
{
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check exists phone or email
        var existedUser = await db.Users.AnyAsync(x => x.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (existedUser)
        {
            throw new AppException("Phone number or Email already registered.");
        }
        
        if (!new EmailAddressAttribute().IsValid(request.Email))
            throw new AppException("Email format invalid");
        
        if (request.Password.Length < 6)
            throw new AppException("Password must be at least 6 characters");

        // Create new user
        var user = new User
        {
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            PassWord = passwordHasher.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Type = UserType.Buyer,
            Status = UserStatus.Active
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return true;
    }
}