using Microsoft.EntityFrameworkCore.Migrations;

namespace programbeaverhut.ru_0._01.Migrations
{
    public partial class IsMarriedToUserAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewPassword23",
                table: "ReportingPeriods");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewPassword23",
                table: "ReportingPeriods",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
