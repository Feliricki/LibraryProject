using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Reviews");
        }
    }
}
