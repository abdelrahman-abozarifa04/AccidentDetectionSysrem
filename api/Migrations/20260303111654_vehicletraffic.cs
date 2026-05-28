using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccidentDetectionSysrem.Migrations
{
    /// <inheritdoc />
    public partial class vehicletraffic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Trafficstatus_TrafficStatusId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Trafficstatus_Cameras_CameraId",
                table: "Trafficstatus");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleDetection_Cameras_CameraId",
                table: "VehicleDetection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleDetection",
                table: "VehicleDetection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trafficstatus",
                table: "Trafficstatus");

            migrationBuilder.RenameTable(
                name: "VehicleDetection",
                newName: "VehicleDetections");

            migrationBuilder.RenameTable(
                name: "Trafficstatus",
                newName: "Trafficstatuses");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleDetection_CameraId",
                table: "VehicleDetections",
                newName: "IX_VehicleDetections_CameraId");

            migrationBuilder.RenameIndex(
                name: "IX_Trafficstatus_CameraId",
                table: "Trafficstatuses",
                newName: "IX_Trafficstatuses_CameraId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleDetections",
                table: "VehicleDetections",
                column: "VehicleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trafficstatuses",
                table: "Trafficstatuses",
                column: "TrafficStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Trafficstatuses_TrafficStatusId",
                table: "Notification",
                column: "TrafficStatusId",
                principalTable: "Trafficstatuses",
                principalColumn: "TrafficStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trafficstatuses_Cameras_CameraId",
                table: "Trafficstatuses",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "CameraId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleDetections_Cameras_CameraId",
                table: "VehicleDetections",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "CameraId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Trafficstatuses_TrafficStatusId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Trafficstatuses_Cameras_CameraId",
                table: "Trafficstatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleDetections_Cameras_CameraId",
                table: "VehicleDetections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleDetections",
                table: "VehicleDetections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trafficstatuses",
                table: "Trafficstatuses");

            migrationBuilder.RenameTable(
                name: "VehicleDetections",
                newName: "VehicleDetection");

            migrationBuilder.RenameTable(
                name: "Trafficstatuses",
                newName: "Trafficstatus");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleDetections_CameraId",
                table: "VehicleDetection",
                newName: "IX_VehicleDetection_CameraId");

            migrationBuilder.RenameIndex(
                name: "IX_Trafficstatuses_CameraId",
                table: "Trafficstatus",
                newName: "IX_Trafficstatus_CameraId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleDetection",
                table: "VehicleDetection",
                column: "VehicleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trafficstatus",
                table: "Trafficstatus",
                column: "TrafficStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Trafficstatus_TrafficStatusId",
                table: "Notification",
                column: "TrafficStatusId",
                principalTable: "Trafficstatus",
                principalColumn: "TrafficStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trafficstatus_Cameras_CameraId",
                table: "Trafficstatus",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "CameraId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleDetection_Cameras_CameraId",
                table: "VehicleDetection",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "CameraId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
