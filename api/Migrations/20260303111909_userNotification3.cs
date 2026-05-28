using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccidentDetectionSysrem.Migrations
{
    /// <inheritdoc />
    public partial class userNotification3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Accidents_AccidentId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Trafficstatuses_TrafficStatusId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifiction_Notification_NotificationId",
                table: "UserNotifiction");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifiction_user_UserId",
                table: "UserNotifiction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNotifiction",
                table: "UserNotifiction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.RenameTable(
                name: "UserNotifiction",
                newName: "UserNotifictions");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameIndex(
                name: "IX_UserNotifiction_NotificationId",
                table: "UserNotifictions",
                newName: "IX_UserNotifictions_NotificationId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_TrafficStatusId",
                table: "Notifications",
                newName: "IX_Notifications_TrafficStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_AccidentId",
                table: "Notifications",
                newName: "IX_Notifications_AccidentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNotifictions",
                table: "UserNotifictions",
                columns: new[] { "UserId", "NotificationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Accidents_AccidentId",
                table: "Notifications",
                column: "AccidentId",
                principalTable: "Accidents",
                principalColumn: "AccidentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Trafficstatuses_TrafficStatusId",
                table: "Notifications",
                column: "TrafficStatusId",
                principalTable: "Trafficstatuses",
                principalColumn: "TrafficStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifictions_Notifications_NotificationId",
                table: "UserNotifictions",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "NotificationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifictions_Users_UserId",
                table: "UserNotifictions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Accidents_AccidentId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Trafficstatuses_TrafficStatusId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifictions_Notifications_NotificationId",
                table: "UserNotifictions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifictions_Users_UserId",
                table: "UserNotifictions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNotifictions",
                table: "UserNotifictions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "user");

            migrationBuilder.RenameTable(
                name: "UserNotifictions",
                newName: "UserNotifiction");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameIndex(
                name: "IX_UserNotifictions_NotificationId",
                table: "UserNotifiction",
                newName: "IX_UserNotifiction_NotificationId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TrafficStatusId",
                table: "Notification",
                newName: "IX_Notification_TrafficStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_AccidentId",
                table: "Notification",
                newName: "IX_Notification_AccidentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                table: "user",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNotifiction",
                table: "UserNotifiction",
                columns: new[] { "UserId", "NotificationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Accidents_AccidentId",
                table: "Notification",
                column: "AccidentId",
                principalTable: "Accidents",
                principalColumn: "AccidentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Trafficstatuses_TrafficStatusId",
                table: "Notification",
                column: "TrafficStatusId",
                principalTable: "Trafficstatuses",
                principalColumn: "TrafficStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifiction_Notification_NotificationId",
                table: "UserNotifiction",
                column: "NotificationId",
                principalTable: "Notification",
                principalColumn: "NotificationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifiction_user_UserId",
                table: "UserNotifiction",
                column: "UserId",
                principalTable: "user",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
