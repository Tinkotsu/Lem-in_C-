using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_FileCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "FileCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FileSize = table.Column<int>(type: "int", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileVersions_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FileCategories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Презентация" });

            migrationBuilder.InsertData(
                table: "FileCategories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Приложение" });

            migrationBuilder.InsertData(
                table: "FileCategories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Другое" });

            migrationBuilder.CreateIndex(
                name: "IX_Files_CategoryId",
                table: "Files",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FileVersions_FileId",
                table: "FileVersions",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileVersions");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "FileCategories");
        }
    }
}
