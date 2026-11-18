using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseRegistration.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOccupiedSeats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OccupiedSeats",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
        UPDATE c
        SET OccupiedSeats = (
            SELECT COUNT(*)
            FROM Registrations r
            WHERE r.CourseId = c.Id
              AND r.Status = 0
        )
        FROM Courses c;
    ");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OccupiedSeats",
                table: "Courses");
        }
    }
}
