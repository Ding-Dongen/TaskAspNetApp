using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskAspNet.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ProjectStatuses",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProjectStatuses",
                newName: "Status");
        }
    }
}
