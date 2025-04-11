using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskAspNet.Data.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    JobTitleId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ProjectStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMembers",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMembers", x => new { x.ProjectId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "ClientName" },
                values: new object[,]
                {
                    { 1, "Acme Corporation" },
                    { 2, "TechStart Inc." },
                    { 3, "Global Solutions Ltd." }
                });

            migrationBuilder.InsertData(
                table: "JobTitles",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { 1, "Developer" },
                    { 2, "Designer" },
                    { 3, "Project Manager" }
                });

            migrationBuilder.InsertData(
                table: "ProjectStatuses",
                columns: new[] { "Id", "StatusName" },
                values: new object[,]
                {
                    { 1, "Started" },
                    { 2, "Completed" }
                });

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "Id", "Address", "City", "DateOfBirth", "Email", "FirstName", "JobTitleId", "LastName", "Phone", "ProfileImageUrl", "ZipCode" },
                values: new object[,]
                {
                    { 1, "123 Main St", "New York", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "John", 1, "Doe", "555-0101", null, "12345" },
                    { 2, "456 Oak Ave", "Los Angeles", new DateTime(1992, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Jane", 2, "Smith", "555-0102", null, "67890" },
                    { 3, "789 Pine Rd", "Chicago", new DateTime(1988, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "mike.johnson@example.com", "Mike", 3, "Johnson", "555-0103", null, "13579" }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Budget", "ClientId", "Description", "EndDate", "ImageUrl", "Name", "StartDate", "StatusId" },
                values: new object[,]
                {
                    { 1, 50000.00m, 1, "Complete overhaul of company website", new DateTime(2024, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Website Redesign", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 75000.00m, 2, "New iOS and Android app development", new DateTime(2024, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Mobile App Development", new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, 100000.00m, 3, "Online store development", new DateTime(2024, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "E-commerce Platform", new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "ProjectMembers",
                columns: new[] { "MemberId", "ProjectId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 2, 2 },
                    { 3, 2 },
                    { 1, 3 },
                    { 3, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_JobTitleId",
                table: "Members",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_MemberId",
                table: "ProjectMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ClientId",
                table: "Projects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_StatusId",
                table: "Projects",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectMembers");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "JobTitles");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "ProjectStatuses");
        }
    }
}
