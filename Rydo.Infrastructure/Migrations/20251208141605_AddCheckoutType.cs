using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rydo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckoutType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CheckoutType",
                table: "PaymentDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckoutType",
                table: "PaymentDetails");
        }
    }
}
