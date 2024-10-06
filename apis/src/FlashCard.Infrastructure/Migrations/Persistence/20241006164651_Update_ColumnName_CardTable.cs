using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCard.Infrastructure.Migrations.Persistence
{
    /// <inheritdoc />
    public partial class Update_ColumnName_CardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Cards",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_Text_Description",
                table: "Cards",
                newName: "IX_Cards_Title_Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Cards",
                newName: "Text");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_Title_Description",
                table: "Cards",
                newName: "IX_Cards_Text_Description");
        }
    }
}
