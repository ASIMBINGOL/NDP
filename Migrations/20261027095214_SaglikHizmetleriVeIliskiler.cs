using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NDPProje.Migrations
{
    /// <inheritdoc />
    public partial class SaglikHizmetleriVeIliskiler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Calisanlar_Id_KurumId",
                table: "Calisanlar",
                columns: new[] { "Id", "KurumId" });

            migrationBuilder.CreateTable(
                name: "SaglikHizmetleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KurumId = table.Column<int>(type: "INTEGER", nullable: false),
                    BolumId = table.Column<int>(type: "INTEGER", nullable: true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SureDakika = table.Column<int>(type: "INTEGER", nullable: false),
                    UcretKurus = table.Column<long>(type: "INTEGER", nullable: false),
                    AktifMi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaglikHizmetleri", x => x.Id);
                    table.UniqueConstraint("AK_SaglikHizmetleri_Id_KurumId", x => new { x.Id, x.KurumId });
                    table.CheckConstraint("CK_SaglikHizmetleri_Sure", "\"SureDakika\" > 0");
                    table.CheckConstraint("CK_SaglikHizmetleri_Ucret", "\"UcretKurus\" >= 0");
                    table.ForeignKey(
                        name: "FK_SaglikHizmetleri_Bolumler_BolumId_KurumId",
                        columns: x => new { x.BolumId, x.KurumId },
                        principalTable: "Bolumler",
                        principalColumns: new[] { "Id", "KurumId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaglikHizmetleri_Kurumlar_KurumId",
                        column: x => x.KurumId,
                        principalTable: "Kurumlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalisanHizmetleri",
                columns: table => new
                {
                    CalisanId = table.Column<int>(type: "INTEGER", nullable: false),
                    SaglikHizmetiId = table.Column<int>(type: "INTEGER", nullable: false),
                    KurumId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalisanHizmetleri", x => new { x.CalisanId, x.SaglikHizmetiId });
                    table.ForeignKey(
                        name: "FK_CalisanHizmetleri_Calisanlar_CalisanId_KurumId",
                        columns: x => new { x.CalisanId, x.KurumId },
                        principalTable: "Calisanlar",
                        principalColumns: new[] { "Id", "KurumId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalisanHizmetleri_SaglikHizmetleri_SaglikHizmetiId_KurumId",
                        columns: x => new { x.SaglikHizmetiId, x.KurumId },
                        principalTable: "SaglikHizmetleri",
                        principalColumns: new[] { "Id", "KurumId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalisanHizmetleri_CalisanId_KurumId",
                table: "CalisanHizmetleri",
                columns: new[] { "CalisanId", "KurumId" });

            migrationBuilder.CreateIndex(
                name: "IX_CalisanHizmetleri_SaglikHizmetiId_KurumId",
                table: "CalisanHizmetleri",
                columns: new[] { "SaglikHizmetiId", "KurumId" });

            migrationBuilder.CreateIndex(
                name: "IX_SaglikHizmetleri_BolumId_KurumId",
                table: "SaglikHizmetleri",
                columns: new[] { "BolumId", "KurumId" });

            migrationBuilder.CreateIndex(
                name: "IX_SaglikHizmetleri_KurumId",
                table: "SaglikHizmetleri",
                column: "KurumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalisanHizmetleri");

            migrationBuilder.DropTable(
                name: "SaglikHizmetleri");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Calisanlar_Id_KurumId",
                table: "Calisanlar");
        }
    }
}
