using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rydo.Appication.Test.Helpers;
using Rydo.Application.Accounts.Commands;
using Rydo.Application.Common.Helpers;
using Rydo.Application.Interfaces.Password;
using Xunit;

namespace Rydo.Appication.Test.IntegrationTests.Accounts.Commands;

public class RegisterCommandHandlerTests : IntegrationTestBase
{
    private readonly TestDbContextFactory _factory;
    private readonly RegisterCommandHandler _handler;
    private readonly Mock<IPasswordHasher> _passwordHasher;

    public RegisterCommandHandlerTests(TestDbContextFactory factory) 
        : base(factory)
    {
        _factory = factory;
        _passwordHasher = new Mock<IPasswordHasher>();
        _passwordHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashedpwd");

        _handler = new RegisterCommandHandler(_factory.DbContext, _passwordHasher.Object);
    }

    [Fact]
    public async Task Should_Create_User_When_Valid()
    {
        var command = new RegisterCommand("0123456789", "123456", "test@gmail.com", "Hoang", "Dao");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();

        var user = await _factory.DbContext.Users.FirstOrDefaultAsync();
        user.Should().NotBeNull();
        user.Email.Should().Be("test@gmail.com");
    }

    [Fact]
    public async Task Should_Throw_When_Phone_Exists()
    {
        // first insert
        await _handler.Handle(new RegisterCommand("012345678", "123456", "test@gmail.com", "Hoang", "Dao"), CancellationToken.None);

        // second insert should fail
        var act = async () => await _handler.Handle(
            new RegisterCommand("012345678", "123456", "another@gmail.com", "Test", "User"),
            CancellationToken.None);

        await act.Should().ThrowAsync<AppException>()
            .WithMessage("Phone number or Email already registered.");
    }
}