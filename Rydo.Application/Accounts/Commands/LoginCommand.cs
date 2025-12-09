using MediatR;
using Rydo.Application.Common.Interfaces;

namespace Rydo.Application.Accounts.Commands;

public class LoginCommand(string UserName, string Password) : IRequest<string>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IApplicationDbContext _db;

    public LoginCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return "";
    }
}
