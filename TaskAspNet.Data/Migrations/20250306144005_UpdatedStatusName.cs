using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskAspNet.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStatusName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProjectStatuses",
                newName: "StatusName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusName",
                table: "ProjectStatuses",
                newName: "Name");
        }
    }
}
