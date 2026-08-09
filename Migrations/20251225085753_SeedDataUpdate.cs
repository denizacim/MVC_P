using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MVC_P.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clubs",
                columns: new[] { "Id", "Aciklama", "Ad", "AktifMi", "KurulusTarihi" },
                values: new object[,]
                {
                    { 1, "Teknoloji ve yazılım", "Bilişim Kulübü", true, new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Spor etkinlikleri", "Spor Kulübü", true, new DateTime(2019, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AdSoyad", "Email", "Rol", "SifreHash" },
                values: new object[] { 1, "Site Admin", "admin@example.com", "Admin", "admin123" });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Aciklama", "BaslangicTarihi", "Baslik", "BitisTarihi", "ClubId", "Durum", "Kontenjan", "Konum" },
                values: new object[,]
                {
                    { 1, ".NET 8 yenilikleri", new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), "C# Semineri", new DateTime(2025, 1, 10, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, "Planlandi", 50, "A Blok Konferans" },
                    { 2, "24 saat kod", new DateTime(2025, 2, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), "Hackathon", new DateTime(2025, 2, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, "Planlandi", 100, "Lab 1" },
                    { 3, "Bahar dönemi", new DateTime(2025, 3, 15, 14, 0, 0, 0, DateTimeKind.Unspecified), "Futbol Turnuvasi", new DateTime(2025, 3, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), 2, "Planlandi", 32, "Saha 2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clubs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clubs",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
