using Microsoft.EntityFrameworkCore.Migrations;

namespace programbeaverhut.ru_0._01.Migrations
{
    public partial class IsMarriedToUserAdded10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewP1234f",
                table: "ReportingPeriods");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewP1234f",
                table: "ReportingPeriods",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
