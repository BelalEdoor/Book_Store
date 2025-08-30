using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShopping_Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixQuantityColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "book",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "book");
        }
    }
}
