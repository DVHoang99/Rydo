using Xunit;

namespace Rydo.Appication.Test.Helpers;

public abstract class IntegrationTestBase : IClassFixture<TestDbContextFactory>
{
    protected readonly TestDbContextFactory Factory;

    protected IntegrationTestBase(TestDbContextFactory factory)
    {
        Factory = factory;
        DatabaseCleaner.ClearDatabase(factory.DbContext).Wait();
    }
}