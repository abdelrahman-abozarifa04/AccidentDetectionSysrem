using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccidentDetectionSysrem.Migrations
{
    public partial class MakeCameraIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CameraId",
                table: "Trafficstatuses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CameraId",
                table: "Accidents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CameraId",
                table: "Trafficstatuses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true,
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CameraId",
                table: "Accidents",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true,
                oldType: "int");
        }
    }
}