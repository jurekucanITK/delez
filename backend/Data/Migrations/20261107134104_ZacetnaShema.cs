using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Delez.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ZacetnaShema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "skupina",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    opis = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skupina", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "uporabnik",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    geslo_hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_uporabnik", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clanstvo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vloga = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tk_uporabnik = table.Column<int>(type: "integer", nullable: false),
                    tk_skupina = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clanstvo", x => x.id);
                    table.CheckConstraint("ck_clanstvo_vloga", "vloga IN ('skrbnik', 'clan', 'omejen')");
                    table.ForeignKey(
                        name: "FK_clanstvo_skupina_tk_skupina",
                        column: x => x.tk_skupina,
                        principalTable: "skupina",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_clanstvo_uporabnik_tk_uporabnik",
                        column: x => x.tk_uporabnik,
                        principalTable: "uporabnik",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "nakazilo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    znesek = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    datum = table.Column<DateOnly>(type: "date", nullable: false),
                    tk_skupina = table.Column<int>(type: "integer", nullable: false),
                    tk_clanstvo_placnik = table.Column<int>(type: "integer", nullable: false),
                    tk_clanstvo_prejemnik = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nakazilo", x => x.id);
                    table.ForeignKey(
                        name: "FK_nakazilo_clanstvo_tk_clanstvo_placnik",
                        column: x => x.tk_clanstvo_placnik,
                        principalTable: "clanstvo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_nakazilo_clanstvo_tk_clanstvo_prejemnik",
                        column: x => x.tk_clanstvo_prejemnik,
                        principalTable: "clanstvo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_nakazilo_skupina_tk_skupina",
                        column: x => x.tk_skupina,
                        principalTable: "skupina",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "strosek",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    opis = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    znesek = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    datum = table.Column<DateOnly>(type: "date", nullable: false),
                    tk_skupina = table.Column<int>(type: "integer", nullable: false),
                    tk_clanstvo_placnik = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_strosek", x => x.id);
                    table.ForeignKey(
                        name: "FK_strosek_clanstvo_tk_clanstvo_placnik",
                        column: x => x.tk_clanstvo_placnik,
                        principalTable: "clanstvo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_strosek_skupina_tk_skupina",
                        column: x => x.tk_skupina,
                        principalTable: "skupina",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "delez_stroska",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    znesek = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    tk_strosek = table.Column<int>(type: "integer", nullable: false),
                    tk_clanstvo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delez_stroska", x => x.id);
                    table.ForeignKey(
                        name: "FK_delez_stroska_clanstvo_tk_clanstvo",
                        column: x => x.tk_clanstvo,
                        principalTable: "clanstvo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_delez_stroska_strosek_tk_strosek",
                        column: x => x.tk_strosek,
                        principalTable: "strosek",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_clanstvo_tk_skupina",
                table: "clanstvo",
                column: "tk_skupina");

            migrationBuilder.CreateIndex(
                name: "IX_clanstvo_tk_uporabnik_tk_skupina",
                table: "clanstvo",
                columns: new[] { "tk_uporabnik", "tk_skupina" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_delez_stroska_tk_clanstvo",
                table: "delez_stroska",
                column: "tk_clanstvo");

            migrationBuilder.CreateIndex(
                name: "IX_delez_stroska_tk_strosek",
                table: "delez_stroska",
                column: "tk_strosek");

            migrationBuilder.CreateIndex(
                name: "IX_nakazilo_tk_clanstvo_placnik",
                table: "nakazilo",
                column: "tk_clanstvo_placnik");

            migrationBuilder.CreateIndex(
                name: "IX_nakazilo_tk_clanstvo_prejemnik",
                table: "nakazilo",
                column: "tk_clanstvo_prejemnik");

            migrationBuilder.CreateIndex(
                name: "IX_nakazilo_tk_skupina",
                table: "nakazilo",
                column: "tk_skupina");

            migrationBuilder.CreateIndex(
                name: "IX_strosek_tk_clanstvo_placnik",
                table: "strosek",
                column: "tk_clanstvo_placnik");

            migrationBuilder.CreateIndex(
                name: "IX_strosek_tk_skupina",
                table: "strosek",
                column: "tk_skupina");

            migrationBuilder.CreateIndex(
                name: "IX_uporabnik_email",
                table: "uporabnik",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delez_stroska");

            migrationBuilder.DropTable(
                name: "nakazilo");

            migrationBuilder.DropTable(
                name: "strosek");

            migrationBuilder.DropTable(
                name: "clanstvo");

            migrationBuilder.DropTable(
                name: "skupina");

            migrationBuilder.DropTable(
                name: "uporabnik");
        }
    }
}
