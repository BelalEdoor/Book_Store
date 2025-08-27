using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShopping_Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdTypeInShoppingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_book_bookid",
                table: "CartDetails");

            migrationBuilder.RenameColumn(
                name: "bookid",
                table: "CartDetails",
                newName: "BookId");

            migrationBuilder.RenameIndex(
                name: "IX_CartDetails_bookid",
                table: "CartDetails",
                newName: "IX_CartDetails_BookId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShoppingCart",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_book_BookId",
                table: "CartDetails",
                column: "BookId",
                principalTable: "book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_book_BookId",
                table: "CartDetails");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "CartDetails",
                newName: "bookid");

            migrationBuilder.RenameIndex(
                name: "IX_CartDetails_BookId",
                table: "CartDetails",
                newName: "IX_CartDetails_bookid");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ShoppingCart",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_book_bookid",
                table: "CartDetails",
                column: "bookid",
                principalTable: "book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
