using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccidentDetectionSysrem.Migrations
{
    /// <inheritdoc />
    public partial class Accident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accident_Cameras_CameraId",
                table: "Accident");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Accident_AccidentId",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accident",
                table: "Accident");

            migrationBuilder.RenameTable(
                name: "Accident",
                newName: "Accidents");

            migrationBuilder.RenameIndex(
                name: "IX_Accident_CameraId",
                table: "Accidents",
                newName: "IX_Accidents_CameraId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accidents",
                table: "Accidents",
                column: "AccidentId");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifiction",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifiction", x => new { x.UserId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_UserNotifiction_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "NotificationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotifiction_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifiction_NotificationId",
                table: "UserNotifiction",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accidents_Cameras_CameraId",
                table: "Accidents",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "CameraId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Accidents_AccidentId",
                table: "Notification",
                column: "AccidentId",
                principalTable: "Accidents",
                principalColumn: "AccidentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accidents_Cameras_CameraId",
                table: "Accidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Accidents_AccidentId",
                table: "Notification");

            migrationBuilder.DropTable(
                name: "UserNotifiction");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accidents",
                table: "Accidents");

            migrationBuilder.RenameTable(
                name: "Accidents",
                newName: "Accident");

            migrationBuilder.RenameIndex(
                name: "IX_Accidents_CameraId",
                table: "Accident",
                newName: "IX_Accident_CameraId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accident",
                table: "Accident",
                column: "AccidentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accident_Cameras_CameraId",
                table: "Accident",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "CameraId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Accident_AccidentId",
                table: "Notification",
                column: "AccidentId",
                principalTable: "Accident",
                principalColumn: "AccidentId");
        }
    }
}
