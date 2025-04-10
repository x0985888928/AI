using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ML.NET.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Customers",
                newName: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Customers",
                newName: "UserName");
        }
    }
}
