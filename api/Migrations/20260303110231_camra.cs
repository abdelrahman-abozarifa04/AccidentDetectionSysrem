using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccidentDetectionSysrem.Migrations
{
    /// <inheritdoc />
    public partial class camra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cameras",
                columns: table => new
                {
                    CameraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CameraName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cameras", x => x.CameraId);
                });

            migrationBuilder.CreateTable(
                name: "Accident",
                columns: table => new
                {
                    AccidentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccidentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CameraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accident", x => x.AccidentId);
                    table.ForeignKey(
                        name: "FK_Accident_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "CameraId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trafficstatus",
                columns: table => new
                {
                    TrafficStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CameraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trafficstatus", x => x.TrafficStatusId);
                    table.ForeignKey(
                        name: "FK_Trafficstatus_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "CameraId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleDetection",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CameraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleDetection", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_VehicleDetection_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "CameraId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccidentId = table.Column<int>(type: "int", nullable: true),
                    TrafficStatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notification_Accident_AccidentId",
                        column: x => x.AccidentId,
                        principalTable: "Accident",
                        principalColumn: "AccidentId");
                    table.ForeignKey(
                        name: "FK_Notification_Trafficstatus_TrafficStatusId",
                        column: x => x.TrafficStatusId,
                        principalTable: "Trafficstatus",
                        principalColumn: "TrafficStatusId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accident_CameraId",
                table: "Accident",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccidentId",
                table: "Notification",
                column: "AccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TrafficStatusId",
                table: "Notification",
                column: "TrafficStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Trafficstatus_CameraId",
                table: "Trafficstatus",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDetection_CameraId",
                table: "VehicleDetection",
                column: "CameraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "VehicleDetection");

            migrationBuilder.DropTable(
                name: "Accident");

            migrationBuilder.DropTable(
                name: "Trafficstatus");

            migrationBuilder.DropTable(
                name: "Cameras");
        }
    }
}
