using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BettingPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleMarketsPerMatchAndTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketSelections_TicketId_MatchId",
                table: "TicketSelections");

            migrationBuilder.DropIndex(
                name: "IX_Offers_MatchId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_OfferOutcomes_OfferId",
                table: "OfferOutcomes");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_MatchId_MarketTemplateId",
                table: "Offers",
                columns: new[] { "MatchId", "MarketTemplateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfferOutcomes_OfferId_OutcomeTemplateId",
                table: "OfferOutcomes",
                columns: new[] { "OfferId", "OutcomeTemplateId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Offers_MatchId_MarketTemplateId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_OfferOutcomes_OfferId_OutcomeTemplateId",
                table: "OfferOutcomes");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSelections_TicketId_MatchId",
                table: "TicketSelections",
                columns: new[] { "TicketId", "MatchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_MatchId",
                table: "Offers",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOutcomes_OfferId",
                table: "OfferOutcomes",
                column: "OfferId");
        }
    }
}
