using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccidentDetectionSysrem.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToTraffic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Trafficstatuses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Trafficstatuses");
        }
    }
}
