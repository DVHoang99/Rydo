using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rydo.Appication.Test.Helpers;
using Rydo.Application.Accounts.Commands;
using Rydo.Application.Common.Helpers;
using Rydo.Application.Interfaces.Password;
using Xunit;

namespace Rydo.Appication.Test.IntegrationTests.Accounts.Commands;

public class UpdateAccountCommandHandlerTests : IntegrationTestBase
{
    private readonly UpdateAccountCommandHandler _handler;
    private readonly TestDbContextFactory _factory;

    public UpdateAccountCommandHandlerTests(TestDbContextFactory factory)
        : base(factory)
    {
        _factory = factory;
        Mock<IPasswordHasher> passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashedpwd");
        _handler = new UpdateAccountCommandHandler(_factory.DbContext, passwordHasher.Object);
    }

    [Fact]
    public async Task Should_Update_User_When_User_Exists()
    {
        // Arrange: tạo user
        var user = new Rydo.Domain.Entities.User
        {
            PhoneNumber = "0123456789",
            PassWord = "oldpwd",
            Email = "old@gmail.com",
            FirstName = "Old",
            LastName = "Name"
        };
        _factory.DbContext.Users.Add(user);
        await _factory.DbContext.SaveChangesAsync();

        var command = new UpdateAccountCommand(
            phoneNumber: "0123456789",
            password: "newpwd",
            email: "new@gmail.com",
            firstName: "New",
            lastName: "User"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        var updatedUser = await _factory.DbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == "0123456789");
        updatedUser.Should().NotBeNull();
        updatedUser.PassWord.Should().Be("hashedpwd");
        updatedUser.Email.Should().Be("new@gmail.com");
        updatedUser.FirstName.Should().Be("New");
        updatedUser.LastName.Should().Be("User");
    }

    [Fact]
    public async Task Should_Throw_AppException_When_User_Not_Exists()
    {
        // Arrange: đảm bảo user không tồn tại
        var command = new UpdateAccountCommand(
            phoneNumber: "0999999999",
            password: "pwd",
            email: "test@gmail.com",
            firstName: "Test",
            lastName: "User"
        );

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AppException>()
            .WithMessage("User with phone number 0999999999 not found.");
    }
}