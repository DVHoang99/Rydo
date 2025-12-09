using MediatR;
using Rydo.Application.Common.Interfaces;

namespace Rydo.Application.Accounts.Commands;

public class CreateAccountCommand(string PhoneNumber, string Password, string FirstName, string LastName, string Email)
    : IRequest<bool>;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, bool>
{
    private readonly IApplicationDbContext _db;
    public CreateAccountCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<bool> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        return true;
    }
}