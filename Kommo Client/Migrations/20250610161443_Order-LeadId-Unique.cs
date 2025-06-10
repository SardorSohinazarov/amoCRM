using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kommo_Client.Migrations
{
    /// <inheritdoc />
    public partial class OrderLeadIdUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orders_LeadId",
                table: "Orders",
                column: "LeadId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_LeadId",
                table: "Orders");
        }
    }
}
