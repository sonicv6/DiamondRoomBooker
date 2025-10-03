using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibCalBooker.Migrations
{
    /// <inheritdoc />
    public partial class dothething : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecurringBookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Interval = table.Column<string>(type: "TEXT", nullable: false),
                    BookingTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RoomID = table.Column<int>(type: "INTEGER", nullable: false),
                    BookerID = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringBookings_AspNetUsers_BookerID",
                        column: x => x.BookerID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringBookings_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBookings_BookerID",
                table: "RecurringBookings",
                column: "BookerID");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBookings_RoomID",
                table: "RecurringBookings",
                column: "RoomID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringBookings");
        }
    }
}
