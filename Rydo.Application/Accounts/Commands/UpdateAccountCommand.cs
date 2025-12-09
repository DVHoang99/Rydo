using MediatR;
using Microsoft.EntityFrameworkCore;
using Rydo.Application.Common.Helpers;
using Rydo.Application.Common.Interfaces;
using Rydo.Application.Interfaces.Password;

namespace Rydo.Application.Accounts.Commands;

public class UpdateAccountCommand(
    string phoneNumber,
    string? password = null,
    string? email = null,
    string? firstName = null,
    string? lastName = null)
    : IRequest<bool>
{
    public string PhoneNumber { get; set; } = phoneNumber;
    public string? Password { get; set; } = password;
    public string? Email { get; set; } = email;
    public string? FirstName { get; set; } = firstName;
    public string? LastName { get; set; } = lastName;
}

public class UpdateAccountCommandHandler(IApplicationDbContext db, IPasswordHasher passwordHasher) : IRequestHandler<UpdateAccountCommand, bool>
{
    public async Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        // Tìm user theo PhoneNumber
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (user == null) 
            throw new AppException($"User with phone number {request.PhoneNumber} not found.");
        
        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PassWord = passwordHasher.Hash(request.Password);

        if (!string.IsNullOrWhiteSpace(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.LastName = request.LastName;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}