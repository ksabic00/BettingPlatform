using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BettingPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AllowTopAndRegularPerMatchMarket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Offers_MatchId_MarketTemplateId",
                table: "Offers");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_MatchId_MarketTemplateId_Category",
                table: "Offers",
                columns: new[] { "MatchId", "MarketTemplateId", "Category" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Offers_MatchId_MarketTemplateId_Category",
                table: "Offers");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_MatchId_MarketTemplateId",
                table: "Offers",
                columns: new[] { "MatchId", "MarketTemplateId" },
                unique: true);
        }
    }
}
