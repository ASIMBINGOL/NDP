using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NDPProje.Migrations
{
    /// <inheritdoc />
    public partial class KurumBolumVeCalismaSaatleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bolumler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KurumId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bolumler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bolumler_Kurumlar_KurumId",
                        column: x => x.KurumId,
                        principalTable: "Kurumlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KurumCalismaSaatleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KurumId = table.Column<int>(type: "INTEGER", nullable: false),
                    HaftaninGunu = table.Column<int>(type: "INTEGER", nullable: false),
                    AcilisSaati = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true),
                    KapanisSaati = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true),
                    KapaliMi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KurumCalismaSaatleri", x => x.Id);
                    table.CheckConstraint("CK_CalismaSaatleri_Gun", "\"HaftaninGunu\" BETWEEN 0 AND 6");
                    table.CheckConstraint("CK_CalismaSaatleri_Saat", "\"KapaliMi\" = 1 OR (\"AcilisSaati\" IS NOT NULL AND \"KapanisSaati\" IS NOT NULL AND \"AcilisSaati\" < \"KapanisSaati\")");
                    table.ForeignKey(
                        name: "FK_KurumCalismaSaatleri_Kurumlar_KurumId",
                        column: x => x.KurumId,
                        principalTable: "Kurumlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_KurumId_Ad",
                table: "Bolumler",
                columns: new[] { "KurumId", "Ad" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KurumCalismaSaatleri_KurumId_HaftaninGunu",
                table: "KurumCalismaSaatleri",
                columns: new[] { "KurumId", "HaftaninGunu" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bolumler");

            migrationBuilder.DropTable(
                name: "KurumCalismaSaatleri");
        }
    }
}
