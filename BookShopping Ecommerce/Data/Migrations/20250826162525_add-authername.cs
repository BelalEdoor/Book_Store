using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShopping_Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class addauthername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AutherName",
                table: "book",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutherName",
                table: "book");
        }
    }
}
