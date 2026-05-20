using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GasWebProject.Migrations
{
    /// <inheritdoc />
    public partial class addColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    M = table.Column<double>(type: "float", nullable: false),
                    Z = table.Column<double>(type: "float", nullable: false),
                    Nominal = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Name);
                });

            migrationBuilder.InsertData(
                table: "Components",
                columns: new[] { "Name", "M", "Nominal", "Z" },
                values: new object[,]
                {
                    { "C2H2", 26.037199999999999, 6.9999999999999999E-06, 0.99270000000000003 },
                    { "C2H4", 28.05376, 4.0000000000000003E-05, 0.99394000000000005 },
                    { "C2H6", 30.06964, 0.00014999999999999999, 0.99197000000000002 },
                    { "C3H8", 44.096519999999998, 1.0000000000000001E-05, 0.98306000000000004 },
                    { "CH4", 16.042760000000001, 0.00050000000000000001, 0.99814000000000003 },
                    { "N2", 28.013480000000001, null, 0.99975999999999998 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Components");
        }
    }
}
