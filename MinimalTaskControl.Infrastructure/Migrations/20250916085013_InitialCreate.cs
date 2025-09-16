using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalTaskControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "task_control_db");

            migrationBuilder.CreateTable(
                name: "tasks",
                schema: "task_control_db",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    author = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    assignee = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<string>(type: "text", nullable: false),
                    parent_task_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tasks", x => x.id);
                    table.ForeignKey(
                        name: "fk_tasks_tasks_parent_task_id",
                        column: x => x.parent_task_id,
                        principalSchema: "task_control_db",
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "task_relations",
                schema: "task_control_db",
                columns: table => new
                {
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    related_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_task_relations", x => new { x.task_id, x.related_task_id });
                    table.ForeignKey(
                        name: "fk_task_relations_tasks_related_task_id",
                        column: x => x.related_task_id,
                        principalSchema: "task_control_db",
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_task_relations_tasks_task_id",
                        column: x => x.task_id,
                        principalSchema: "task_control_db",
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_task_relations_related_task_id",
                schema: "task_control_db",
                table: "task_relations",
                column: "related_task_id");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_assignee",
                schema: "task_control_db",
                table: "tasks",
                column: "assignee");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_author",
                schema: "task_control_db",
                table: "tasks",
                column: "author");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_parent_task_id",
                schema: "task_control_db",
                table: "tasks",
                column: "parent_task_id");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_priority",
                schema: "task_control_db",
                table: "tasks",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_status",
                schema: "task_control_db",
                table: "tasks",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "task_relations",
                schema: "task_control_db");

            migrationBuilder.DropTable(
                name: "tasks",
                schema: "task_control_db");
        }
    }
}
