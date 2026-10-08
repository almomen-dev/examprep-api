using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examprep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Questions");
        }
    }
}
