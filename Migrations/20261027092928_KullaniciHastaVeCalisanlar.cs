using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NDPProje.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciHastaVeCalisanlar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Bolumler_Id_KurumId",
                table: "Bolumler",
                columns: new[] { "Id", "KurumId" });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdSoyad = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Eposta = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false, collation: "NOCASE"),
                    SifreOzeti = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    AktifMi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.CheckConstraint("CK_Kullanicilar_Rol", "\"Rol\" IN ('Hasta', 'Doktor', 'SaglikPersoneli', 'KurumYoneticisi')");
                });

            migrationBuilder.CreateTable(
                name: "Calisanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    KurumId = table.Column<int>(type: "INTEGER", nullable: false),
                    BolumId = table.Column<int>(type: "INTEGER", nullable: true),
                    AktifMi = table.Column<bool>(type: "INTEGER", nullable: false),
                    CalisanTuru = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    UzmanlikAlani = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Gorev = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calisanlar", x => x.Id);
                    table.CheckConstraint("CK_Calisanlar_Turu", "\"CalisanTuru\" IN ('Doktor', 'SaglikPersoneli')");
                    table.ForeignKey(
                        name: "FK_Calisanlar_Bolumler_BolumId_KurumId",
                        columns: x => new { x.BolumId, x.KurumId },
                        principalTable: "Bolumler",
                        principalColumns: new[] { "Id", "KurumId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Calisanlar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Calisanlar_Kurumlar_KurumId",
                        column: x => x.KurumId,
                        principalTable: "Kurumlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Hastalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    Telefon = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    DogumTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hastalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hastalar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calisanlar_BolumId_KurumId",
                table: "Calisanlar",
                columns: new[] { "BolumId", "KurumId" });

            migrationBuilder.CreateIndex(
                name: "IX_Calisanlar_KullaniciId_KurumId",
                table: "Calisanlar",
                columns: new[] { "KullaniciId", "KurumId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calisanlar_KurumId",
                table: "Calisanlar",
                column: "KurumId");

            migrationBuilder.CreateIndex(
                name: "IX_Hastalar_KullaniciId",
                table: "Hastalar",
                column: "KullaniciId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_Eposta",
                table: "Kullanicilar",
                column: "Eposta",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calisanlar");

            migrationBuilder.DropTable(
                name: "Hastalar");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Bolumler_Id_KurumId",
                table: "Bolumler");
        }
    }
}
