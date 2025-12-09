using Microsoft.EntityFrameworkCore;
using Rydo.Application.Common.Interfaces;
using Rydo.Infrastructure.Persistence;

namespace Rydo.Appication.Test.Helpers;

public static class DatabaseCleaner
{
    public static async Task ClearDatabase(ApplicationDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            DO $$
            DECLARE
                table_name text;
            BEGIN
                FOR table_name IN 
                    SELECT tablename FROM pg_tables WHERE schemaname = 'public'
                LOOP
                    EXECUTE 'TRUNCATE TABLE ' || quote_ident(table_name) || ' RESTART IDENTITY CASCADE;';
                END LOOP;
            END $$;
        ");
    }
}