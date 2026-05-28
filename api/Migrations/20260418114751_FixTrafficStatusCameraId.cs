using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccidentDetectionSysrem.Migrations
{
    /// <inheritdoc />
    public partial class FixTrafficStatusCameraId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trafficstatuses_Cameras_CameraId",
                table: "Trafficstatuses");

            migrationBuilder.AlterColumn<int>(
                name: "CameraId",
                table: "Trafficstatuses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Trafficstatuses_Cameras_CameraId",
                table: "Trafficstatuses",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "CameraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trafficstatuses_Cameras_CameraId",
                table: "Trafficstatuses");

            migrationBuilder.AlterColumn<int>(
                name: "CameraId",
                table: "Trafficstatuses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trafficstatuses_Cameras_CameraId",
                table: "Trafficstatuses",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "CameraId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
