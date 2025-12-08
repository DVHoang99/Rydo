using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rydo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE ""PaymentDetails""
            RENAME COLUMN ""DetailId"" TO ""Detail"";
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE ""PaymentDetails""
            RENAME COLUMN ""Detail"" TO ""DetailId"";
        ");
        }
    }
}
