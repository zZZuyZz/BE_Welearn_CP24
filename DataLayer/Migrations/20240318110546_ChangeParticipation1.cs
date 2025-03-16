using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class ChangeParticipation1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MeetingParticipations",
                table: "MeetingParticipations");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MeetingParticipations",
                newName: "SinganlrId");

            migrationBuilder.AddColumn<int>(
                name: "MeetingParticipationsId",
                table: "MeetingParticipations",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "PeerId",
                table: "MeetingParticipations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeetingParticipations",
                table: "MeetingParticipations",
                column: "MeetingParticipationsId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingParticipations_SinganlrId",
                table: "MeetingParticipations",
                column: "SinganlrId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MeetingParticipations",
                table: "MeetingParticipations");

            migrationBuilder.DropIndex(
                name: "IX_MeetingParticipations_SinganlrId",
                table: "MeetingParticipations");

            migrationBuilder.DropColumn(
                name: "MeetingParticipationsId",
                table: "MeetingParticipations");

            migrationBuilder.DropColumn(
                name: "PeerId",
                table: "MeetingParticipations");

            migrationBuilder.RenameColumn(
                name: "SinganlrId",
                table: "MeetingParticipations",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeetingParticipations",
                table: "MeetingParticipations",
                column: "Id");
        }
    }
}
