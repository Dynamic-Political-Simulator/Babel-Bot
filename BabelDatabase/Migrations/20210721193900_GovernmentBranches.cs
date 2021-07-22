using Microsoft.EntityFrameworkCore.Migrations;

namespace BabelDatabase.Migrations
{
    public partial class GovernmentBranches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GovernmentBranches",
                columns: table => new
                {
                    GovernmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerceivedAlignmentAlignmentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Modifiers = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GovernmentBranches", x => x.GovernmentId);
                    table.ForeignKey(
                        name: "FK_GovernmentBranches_Alignments_PerceivedAlignmentAlignmentId",
                        column: x => x.PerceivedAlignmentAlignmentId,
                        principalTable: "Alignments",
                        principalColumn: "AlignmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GovernmentBranches_PerceivedAlignmentAlignmentId",
                table: "GovernmentBranches",
                column: "PerceivedAlignmentAlignmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GovernmentBranches");
        }
    }
}
