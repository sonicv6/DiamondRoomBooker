using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibCalBooker.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringFieldtoBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Interval",
                table: "Bookings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Recurring",
                table: "Bookings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interval",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Recurring",
                table: "Bookings");
        }
    }
}
