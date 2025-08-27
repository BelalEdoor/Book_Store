using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShopping_Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRequiredFromGenreNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AutherName",
                table: "book",
                newName: "AuthorName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorName",
                table: "book",
                newName: "AutherName");
        }
    }
}
