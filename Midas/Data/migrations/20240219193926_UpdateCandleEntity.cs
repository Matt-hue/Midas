using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Midas.data.migrations
{
    /// <inheritdoc />
    public partial class UpdateCandleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Interval",
                table: "Candle",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interval",
                table: "Candle");
        }
    }
}
