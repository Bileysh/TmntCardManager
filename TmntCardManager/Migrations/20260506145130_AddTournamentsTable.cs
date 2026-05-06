using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TmntCardManager.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TournamentId",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tournaments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    PrizeCoins = table.Column<int>(type: "integer", nullable: false),
                    startedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    endedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tournaments_pkey", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_matches_TournamentId",
                table: "matches",
                column: "TournamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_tournaments_TournamentId",
                table: "matches",
                column: "TournamentId",
                principalTable: "tournaments",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_tournaments_TournamentId",
                table: "matches");

            migrationBuilder.DropTable(
                name: "tournaments");

            migrationBuilder.DropIndex(
                name: "IX_matches_TournamentId",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "TournamentId",
                table: "matches");
        }
    }
}
