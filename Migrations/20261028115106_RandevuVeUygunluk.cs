using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NDPProje.Migrations
{
    /// <inheritdoc />
    public partial class RandevuVeUygunluk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UygunlukAraliklari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CalisanId = table.Column<int>(type: "INTEGER", nullable: false),
                    KurumId = table.Column<int>(type: "INTEGER", nullable: false),
                    BaslangicTarihiSaati = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BitisTarihiSaati = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UygunlukAraliklari", x => x.Id);
                    table.UniqueConstraint("AK_UygunlukAraliklari_Id_CalisanId_KurumId", x => new { x.Id, x.CalisanId, x.KurumId });
                    table.CheckConstraint("CK_UygunlukAraliklari_Tarih", "\"BitisTarihiSaati\" > \"BaslangicTarihiSaati\"");
                    table.ForeignKey(
                        name: "FK_UygunlukAraliklari_Calisanlar_CalisanId_KurumId",
                        columns: x => new { x.CalisanId, x.KurumId },
                        principalTable: "Calisanlar",
                        principalColumns: new[] { "Id", "KurumId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Randevular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HastaId = table.Column<int>(type: "INTEGER", nullable: false),
                    CalisanId = table.Column<int>(type: "INTEGER", nullable: false),
                    SaglikHizmetiId = table.Column<int>(type: "INTEGER", nullable: false),
                    KurumId = table.Column<int>(type: "INTEGER", nullable: false),
                    UygunlukAraligiId = table.Column<int>(type: "INTEGER", nullable: false),
                    BaslangicTarihiSaati = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BitisTarihiSaati = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SureDakikaKayit = table.Column<int>(type: "INTEGER", nullable: false),
                    UcretKurusKayit = table.Column<long>(type: "INTEGER", nullable: false),
                    Durum = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Randevular", x => x.Id);
                    table.CheckConstraint("CK_Randevular_Durum", "\"Durum\" IN ('Beklemede', 'Onaylandi', 'IptalEdildi', 'Tamamlandi')");
                    table.CheckConstraint("CK_Randevular_Sure", "\"SureDakikaKayit\" > 0");
                    table.CheckConstraint("CK_Randevular_Tarih", "\"BitisTarihiSaati\" > \"BaslangicTarihiSaati\"");
                    table.CheckConstraint("CK_Randevular_Ucret", "\"UcretKurusKayit\" >= 0");
                    table.ForeignKey(
                        name: "FK_Randevular_CalisanHizmetleri_CalisanId_SaglikHizmetiId",
                        columns: x => new { x.CalisanId, x.SaglikHizmetiId },
                        principalTable: "CalisanHizmetleri",
                        principalColumns: new[] { "CalisanId", "SaglikHizmetiId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Randevular_Calisanlar_CalisanId_KurumId",
                        columns: x => new { x.CalisanId, x.KurumId },
                        principalTable: "Calisanlar",
                        principalColumns: new[] { "Id", "KurumId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Randevular_Hastalar_HastaId",
                        column: x => x.HastaId,
                        principalTable: "Hastalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Randevular_SaglikHizmetleri_SaglikHizmetiId_KurumId",
                        columns: x => new { x.SaglikHizmetiId, x.KurumId },
                        principalTable: "SaglikHizmetleri",
                        principalColumns: new[] { "Id", "KurumId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Randevular_UygunlukAraliklari_UygunlukAraligiId_CalisanId_KurumId",
                        columns: x => new { x.UygunlukAraligiId, x.CalisanId, x.KurumId },
                        principalTable: "UygunlukAraliklari",
                        principalColumns: new[] { "Id", "CalisanId", "KurumId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_CalisanId_BaslangicTarihiSaati_BitisTarihiSaati",
                table: "Randevular",
                columns: new[] { "CalisanId", "BaslangicTarihiSaati", "BitisTarihiSaati" });

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_CalisanId_KurumId",
                table: "Randevular",
                columns: new[] { "CalisanId", "KurumId" });

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_CalisanId_SaglikHizmetiId",
                table: "Randevular",
                columns: new[] { "CalisanId", "SaglikHizmetiId" });

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_HastaId_BaslangicTarihiSaati",
                table: "Randevular",
                columns: new[] { "HastaId", "BaslangicTarihiSaati" });

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_SaglikHizmetiId_KurumId",
                table: "Randevular",
                columns: new[] { "SaglikHizmetiId", "KurumId" });

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_UygunlukAraligiId_CalisanId_KurumId",
                table: "Randevular",
                columns: new[] { "UygunlukAraligiId", "CalisanId", "KurumId" });

            migrationBuilder.CreateIndex(
                name: "IX_UygunlukAraliklari_CalisanId_BaslangicTarihiSaati_BitisTarihiSaati",
                table: "UygunlukAraliklari",
                columns: new[] { "CalisanId", "BaslangicTarihiSaati", "BitisTarihiSaati" });

            migrationBuilder.CreateIndex(
                name: "IX_UygunlukAraliklari_CalisanId_KurumId",
                table: "UygunlukAraliklari",
                columns: new[] { "CalisanId", "KurumId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Randevular");

            migrationBuilder.DropTable(
                name: "UygunlukAraliklari");
        }
    }
}
