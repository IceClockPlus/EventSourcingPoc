using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventSourcingPoc.API.EFContext.Migrations
{
    /// <inheritdoc />
    public partial class InsuranceCounterColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "certificate_number_counter",
                table: "insurances",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "certificate_number_counter",
                table: "insurances");
        }
    }
}
