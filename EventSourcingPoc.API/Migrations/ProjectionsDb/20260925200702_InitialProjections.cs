using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventSourcingPoc.API.Migrations.ProjectionsDb
{
    /// <inheritdoc />
    public partial class InitialProjections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerSummaries",
                columns: table => new
                {
                    TaxId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StreamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsedBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSummaries", x => x.TaxId);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionCheckpoints",
                columns: table => new
                {
                    ProjectionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastProcessedSequenceNumber = table.Column<long>(type: "bigint", nullable: false),
                    LastEventAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionCheckpoints", x => x.ProjectionName);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSummaries_StreamId",
                table: "CustomerSummaries",
                column: "StreamId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerSummaries");

            migrationBuilder.DropTable(
                name: "ProjectionCheckpoints");
        }
    }
}
