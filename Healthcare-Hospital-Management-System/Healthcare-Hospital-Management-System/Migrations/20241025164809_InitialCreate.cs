using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Healthcare_Hospital_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DrugReports",
                columns: table => new
                {
                    SafetyReportId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReportDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PrimarySourceCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Serious = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReporterQualification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PatientGender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Reactions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderOrganization = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReceiverOrganization = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugReports", x => x.SafetyReportId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrugReports");
        }
    }
}
