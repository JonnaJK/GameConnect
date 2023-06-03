using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChatMessageStatusUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessageStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatMessageId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    RecipientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessageStatus", x => x.Id);
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessageStatus");

        }
    }
}
