using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentinel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DeviceType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastHeartbeat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Metadata = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "log_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Source = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    StackTrace = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    Properties = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_log_entries_agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LogEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RiskScore = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AiExplanation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "boolean", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AcknowledgedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.Id);
                    table.CheckConstraint("CK_Alert_RiskScore", "\"RiskScore\" >= 1 AND \"RiskScore\" <= 10");
                    table.ForeignKey(
                        name: "FK_alerts_log_entries_LogEntryId",
                        column: x => x.LogEntryId,
                        principalTable: "log_entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agents_Name",
                table: "agents",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alerts_CreatedAt",
                table: "alerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_LogEntryId",
                table: "alerts",
                column: "LogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_Severity_IsAcknowledged",
                table: "alerts",
                columns: new[] { "Severity", "IsAcknowledged" });

            migrationBuilder.CreateIndex(
                name: "IX_log_entries_AgentId_Timestamp",
                table: "log_entries",
                columns: new[] { "AgentId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_log_entries_Level",
                table: "log_entries",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_log_entries_Timestamp",
                table: "log_entries",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.DropTable(
                name: "log_entries");

            migrationBuilder.DropTable(
                name: "agents");
        }
    }
}
