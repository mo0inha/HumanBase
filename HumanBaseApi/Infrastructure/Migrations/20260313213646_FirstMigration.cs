using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    DS_DESCRIPTION = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ST_TYPE_FINANCIAL = table.Column<byte>(type: "smallint", nullable: false),
                    ST_ACTIVE = table.Column<bool>(type: "boolean", nullable: false),
                    ST_IS_DELETED = table.Column<bool>(type: "boolean", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    DS_NAME = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    DT_BIRTHDAY = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ST_ACTIVE = table.Column<bool>(type: "boolean", nullable: false),
                    ST_IS_DELETED = table.Column<bool>(type: "boolean", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AppTransaction",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    DS_DESCRIPTION = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NR_VALUE = table.Column<decimal>(type: "numeric", nullable: false),
                    ST_TYPE_FINANCIAL = table.Column<byte>(type: "smallint", nullable: false),
                    ID_CATEGORY = table.Column<Guid>(type: "uuid", nullable: false),
                    ID_PERSON = table.Column<Guid>(type: "uuid", nullable: false),
                    ST_ACTIVE = table.Column<bool>(type: "boolean", nullable: false),
                    ST_IS_DELETED = table.Column<bool>(type: "boolean", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTransaction", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AppTransaction_Category_ID_CATEGORY",
                        column: x => x.ID_CATEGORY,
                        principalTable: "Category",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppTransaction_Person_ID_PERSON",
                        column: x => x.ID_PERSON,
                        principalTable: "Person",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTransaction_ID_CATEGORY",
                table: "AppTransaction",
                column: "ID_CATEGORY");

            migrationBuilder.CreateIndex(
                name: "IX_AppTransaction_ID_PERSON",
                table: "AppTransaction",
                column: "ID_PERSON");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTransaction");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Person");
        }
    }
}
