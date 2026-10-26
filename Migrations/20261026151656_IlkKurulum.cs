using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NDPProje.Migrations
{
    /// <inheritdoc />
    public partial class IlkKurulum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kurumlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UstKurumId = table.Column<int>(type: "INTEGER", nullable: true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    KurumTuru = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Adres = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Telefon = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    AktifMi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kurumlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kurumlar_Kurumlar_UstKurumId",
                        column: x => x.UstKurumId,
                        principalTable: "Kurumlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kurumlar_UstKurumId",
                table: "Kurumlar",
                column: "UstKurumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kurumlar");
        }
    }
}
