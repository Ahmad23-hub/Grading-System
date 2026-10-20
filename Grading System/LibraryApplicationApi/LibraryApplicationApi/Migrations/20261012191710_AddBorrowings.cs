using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryApplicationApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Members",
                newName: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Members",
                newName: "id");
        }
    }
}
