using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BettingPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    HomeTeam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AwayTeam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sport = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "football"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutcomeTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomeTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarketTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ValidFromUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidToUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offers_MarketTemplates_MarketTemplateId",
                        column: x => x.MarketTemplateId,
                        principalTable: "MarketTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offers_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketTemplateOutcomes",
                columns: table => new
                {
                    MarketTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutcomeTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketTemplateOutcomes", x => new { x.MarketTemplateId, x.OutcomeTemplateId });
                    table.ForeignKey(
                        name: "FK_MarketTemplateOutcomes_MarketTemplates_MarketTemplateId",
                        column: x => x.MarketTemplateId,
                        principalTable: "MarketTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketTemplateOutcomes_OutcomeTemplates_OutcomeTemplateId",
                        column: x => x.OutcomeTemplateId,
                        principalTable: "OutcomeTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StakeGross = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    FeePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeeAmount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    StakeNet = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    CombinedOdds = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PotentialPayout = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PayoutAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfferOutcomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutcomeTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Odds = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferOutcomes", x => x.Id);
                    table.CheckConstraint("CK_OfferOutcome_Odds_Min", "[Odds] >= 1.00");
                    table.ForeignKey(
                        name: "FK_OfferOutcomes_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferOutcomes_OutcomeTemplates_OutcomeTemplateId",
                        column: x => x.OutcomeTemplateId,
                        principalTable: "OutcomeTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketSelections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutcomeTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OddAtPlacement = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    CategoryAtPlacement = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketSelections_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketSelections_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketSelections_OutcomeTemplates_OutcomeTemplateId",
                        column: x => x.OutcomeTemplateId,
                        principalTable: "OutcomeTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketSelections_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketTemplateOutcomes_OutcomeTemplateId",
                table: "MarketTemplateOutcomes",
                column: "OutcomeTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketTemplates_Code",
                table: "MarketTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfferOutcomes_OfferId",
                table: "OfferOutcomes",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOutcomes_OutcomeTemplateId",
                table: "OfferOutcomes",
                column: "OutcomeTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_MarketTemplateId",
                table: "Offers",
                column: "MarketTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_MatchId",
                table: "Offers",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeTemplates_Code",
                table: "OutcomeTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PlayerId",
                table: "Tickets",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSelection_TopOnly",
                table: "TicketSelections",
                column: "TicketId",
                unique: true,
                filter: "[CategoryAtPlacement] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSelections_MatchId",
                table: "TicketSelections",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSelections_OfferId",
                table: "TicketSelections",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSelections_OutcomeTemplateId",
                table: "TicketSelections",
                column: "OutcomeTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSelections_TicketId_MatchId",
                table: "TicketSelections",
                columns: new[] { "TicketId", "MatchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_PlayerId",
                table: "Wallets",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketTemplateOutcomes");

            migrationBuilder.DropTable(
                name: "OfferOutcomes");

            migrationBuilder.DropTable(
                name: "TicketSelections");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "OutcomeTemplates");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "MarketTemplates");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
