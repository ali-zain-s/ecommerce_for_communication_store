using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAdminVerified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdminVerified",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminVerified",
                table: "Orders");
        }
    }
}
