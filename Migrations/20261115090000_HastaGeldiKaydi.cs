using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NDPProje.Migrations;

public partial class HastaGeldiKaydi : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "HastaGelisTarihiSaati",
            table: "Randevular",
            type: "TEXT",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "HastaGelisTarihiSaati",
            table: "Randevular");
    }
}
