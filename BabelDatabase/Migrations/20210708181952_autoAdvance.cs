using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BabelDatabase.Migrations
{
    public partial class autoAdvance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutoAdvance",
                columns: table => new
                {
                    AutoAdvanceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    DayExceptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountOfYears = table.Column<int>(type: "int", nullable: false),
                    LastDayTriggered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChannelId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoAdvance", x => x.AutoAdvanceId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoAdvance");
        }
    }
}
