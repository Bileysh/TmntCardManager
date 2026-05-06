using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TmntCardManager.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player1id = table.Column<int>(type: "integer", nullable: false),
                    player2id = table.Column<int>(type: "integer", nullable: false),
                    winnerid = table.Column<int>(type: "integer", nullable: true),
                    playedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("matches_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_matches_player1",
                        column: x => x.player1id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_matches_player2",
                        column: x => x.player2id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_matches_winner",
                        column: x => x.winnerid,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_matches_player1id",
                table: "matches",
                column: "player1id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_player2id",
                table: "matches",
                column: "player2id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_winnerid",
                table: "matches",
                column: "winnerid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matches");
        }
    }
}
