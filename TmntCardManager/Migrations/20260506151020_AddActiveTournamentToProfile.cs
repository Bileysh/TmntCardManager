using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TmntCardManager.Migrations
{
    /// <inheritdoc />
    public partial class AddActiveTournamentToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveTournamentId",
                table: "playerprofiles",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveTournamentId",
                table: "playerprofiles");
        }
    }
}
