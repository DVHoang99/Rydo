using Microsoft.EntityFrameworkCore;
using Rydo.Application.Common.Interfaces;
using Rydo.Infrastructure.Persistence;

namespace Rydo.Appication.Test.Helpers;

public static class DatabaseCleaner
{
    public static async Task ClearDatabase(ApplicationDbContext db)
    {
        var tables = new[] { "PaymentDetails", "Details", "Users" };

        foreach (var table in tables)
        {
            await db.Database.ExecuteSqlRawAsync($@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT FROM information_schema.tables 
                               WHERE table_name = '{table}') THEN
                        EXECUTE 'TRUNCATE TABLE ""{table}"" RESTART IDENTITY CASCADE';
                    END IF;
                END $$;");
        }
    }
}