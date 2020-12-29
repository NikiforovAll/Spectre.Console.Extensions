using Microsoft.EntityFrameworkCore.Migrations;

namespace DataTableEfCore.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Composers",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Composers", x => x.PersonId);
                });

            migrationBuilder.InsertData(
                table: "Composers",
                columns: new[] { "PersonId", "Age", "FirstName", "LastName" },
                values: new object[] { 1, 65, "Sebastian", "Bach" });

            migrationBuilder.InsertData(
                table: "Composers",
                columns: new[] { "PersonId", "Age", "FirstName", "LastName" },
                values: new object[] { 2, 56, "Ludwig", "Beethoven" });

            migrationBuilder.InsertData(
                table: "Composers",
                columns: new[] { "PersonId", "Age", "FirstName", "LastName" },
                values: new object[] { 3, 31, "Franz", "Schubert" });

            migrationBuilder.InsertData(
                table: "Composers",
                columns: new[] { "PersonId", "Age", "FirstName", "LastName" },
                values: new object[] { 4, 77, "Joseph", "Haydn" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Composers");
        }
    }
}
