using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Kaszinó_projekt.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FelhasználóAdatok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nev = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Jelszo = table.Column<string>(type: "longtext", nullable: false),
                    Eletkor = table.Column<int>(type: "int", nullable: false),
                    Egyenleg = table.Column<int>(type: "int", nullable: false),
                    Admin = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FelhasználóAdatok", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "KaszinóBevitel",
                columns: table => new
                {
                    TranzakcióId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FelhasználóId = table.Column<int>(type: "int", nullable: false),
                    Dátum = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Bevitel = table.Column<int>(type: "int", nullable: false),
                    Kiadás = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KaszinóBevitel", x => x.TranzakcióId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JátékokAdatai",
                columns: table => new
                {
                    JátékId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FelhasznaloId = table.Column<int>(type: "int", nullable: false),
                    JatekTipus = table.Column<string>(type: "longtext", nullable: false),
                    Tet = table.Column<int>(type: "int", nullable: false),
                    Nyeremeny = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Nyerovonalak = table.Column<int>(type: "int", nullable: true),
                    EgyenlegJatekElott = table.Column<int>(type: "int", nullable: false),
                    EgyenlegJatekUtan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JátékokAdatai", x => x.JátékId);
                    table.ForeignKey(
                        name: "FK_JátékokAdatai_FelhasználóAdatok_FelhasznaloId",
                        column: x => x.FelhasznaloId,
                        principalTable: "FelhasználóAdatok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_JátékokAdatai_FelhasznaloId",
                table: "JátékokAdatai",
                column: "FelhasznaloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JátékokAdatai");

            migrationBuilder.DropTable(
                name: "KaszinóBevitel");

            migrationBuilder.DropTable(
                name: "FelhasználóAdatok");
        }
    }
}
