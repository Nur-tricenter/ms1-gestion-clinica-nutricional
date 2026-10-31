using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionClinicaNutricional.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "outbox");

            migrationBuilder.CreateTable(
                name: "outboxMessage",
                schema: "outbox",
                columns: table => new
                {
                    outboxId = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed = table.Column<bool>(type: "boolean", nullable: false),
                    processedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    correlationId = table.Column<string>(type: "text", nullable: true),
                    traceId = table.Column<string>(type: "text", nullable: true),
                    spanId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outboxMessage", x => x.outboxId);
                });

            migrationBuilder.CreateTable(
                name: "Paciente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CI = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paciente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConsultaInicial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Peso = table.Column<int>(type: "integer", nullable: false),
                    Altura = table.Column<double>(type: "double precision", nullable: false),
                    Composicion = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Antecedentes = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultaInicial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultaInicial_Paciente_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Paciente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanAlimenticio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    DuracionPlan = table.Column<int>(type: "integer", nullable: false),
                    EstadoPlan = table.Column<int>(type: "integer", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaSiguienteControl = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanAlimenticio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanAlimenticio_Paciente_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Paciente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HabitoAlimenticio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    ConsultaInicialId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitoAlimenticio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitoAlimenticio_ConsultaInicial_ConsultaInicialId",
                        column: x => x.ConsultaInicialId,
                        principalTable: "ConsultaInicial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlanAlimenticioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluacion_PlanAlimenticio_PlanAlimenticioId",
                        column: x => x.PlanAlimenticioId,
                        principalTable: "PlanAlimenticio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanComida",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    PlanAlimenticioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanComida", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanComida_PlanAlimenticio_PlanAlimenticioId",
                        column: x => x.PlanAlimenticioId,
                        principalTable: "PlanAlimenticio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultaInicial_PacienteId",
                table: "ConsultaInicial",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluacion_PlanAlimenticioId",
                table: "Evaluacion",
                column: "PlanAlimenticioId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitoAlimenticio_ConsultaInicialId",
                table: "HabitoAlimenticio",
                column: "ConsultaInicialId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAlimenticio_PacienteId",
                table: "PlanAlimenticio",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanComida_PlanAlimenticioId",
                table: "PlanComida",
                column: "PlanAlimenticioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evaluacion");

            migrationBuilder.DropTable(
                name: "HabitoAlimenticio");

            migrationBuilder.DropTable(
                name: "outboxMessage",
                schema: "outbox");

            migrationBuilder.DropTable(
                name: "PlanComida");

            migrationBuilder.DropTable(
                name: "ConsultaInicial");

            migrationBuilder.DropTable(
                name: "PlanAlimenticio");

            migrationBuilder.DropTable(
                name: "Paciente");
        }
    }
}
