﻿using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Nebula.Data.Migrations
{
    public partial class RenameCompanyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ruc = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    RznSocial = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CodLocalEmisor = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TipMoneda = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PorcentajeIgv = table.Column<decimal>(type: "numeric", nullable: true),
                    ValorImpuestoBolsa = table.Column<decimal>(type: "numeric", nullable: true),
                    CompletarDatosBoleta = table.Column<decimal>(type: "numeric", nullable: true),
                    CuentaBancoDetraccion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TextoDetraccion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    MontoDetraccion = table.Column<decimal>(type: "numeric", nullable: true),
                    ContactId = table.Column<int>(type: "integer", nullable: true),
                    SoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodLocalEmisor = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CompletarDatosBoleta = table.Column<decimal>(type: "numeric", nullable: true),
                    ContactId = table.Column<int>(type: "integer", nullable: true),
                    CuentaBancoDetraccion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    MontoDetraccion = table.Column<decimal>(type: "numeric", nullable: true),
                    PorcentajeIgv = table.Column<decimal>(type: "numeric", nullable: true),
                    Ruc = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    RznSocial = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TextoDetraccion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TipMoneda = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ValorImpuestoBolsa = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });
        }
    }
}