using MediatR;
using Rydo.Application.Common.Interfaces;

namespace Rydo.Application.Accounts.Commands;

public class UpdateAccountCommand(string PhoneNumber, string Password, string Email, string FirstName, string LastName) : IRequest<bool>;

public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, bool>
{
    private readonly IApplicationDbContext _db;
    public UpdateAccountCommandHandler(IApplicationDbContext db) => _db = db;
    public async Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        return true;
    }
}