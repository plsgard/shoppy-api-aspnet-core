using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Shoppy.Data.Migrations
{
    public partial class Create_Share : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shares",
                columns: table => new
                {
                    ListId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shares", x => new { x.ListId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Shares_Lists_ListId",
                        column: x => x.ListId,
                        principalTable: "Lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shares_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lists_UserId",
                table: "Lists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_UserId",
                table: "Shares",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lists_AspNetUsers_UserId",
                table: "Lists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lists_AspNetUsers_UserId",
                table: "Lists");

            migrationBuilder.DropTable(
                name: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Lists_UserId",
                table: "Lists");
        }
    }
}
