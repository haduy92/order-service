using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCard.Infrastructure.Migrations.Persistence
{
    /// <inheritdoc />
    public partial class Add_FullTextSearch_CardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cards_Text_Description",
                table: "Cards",
                columns: new[] { "Text", "Description" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cards_Text_Description",
                table: "Cards");
        }
    }
}
