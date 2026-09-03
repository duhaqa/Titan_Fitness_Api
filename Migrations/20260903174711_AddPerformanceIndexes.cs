using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Titan_Fitness.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Trainers_IsActive",
                table: "Trainers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_Status",
                table: "Memberships",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_SessionDate",
                table: "ClassSessions",
                column: "SessionDate");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_CheckInDateTime",
                table: "CheckIns",
                column: "CheckInDateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trainers_IsActive",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_Status",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_ClassSessions_SessionDate",
                table: "ClassSessions");

            migrationBuilder.DropIndex(
                name: "IX_CheckIns_CheckInDateTime",
                table: "CheckIns");
        }
    }
}
