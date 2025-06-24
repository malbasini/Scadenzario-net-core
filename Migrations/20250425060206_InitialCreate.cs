
using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scadenzario.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    FullName = table.Column<string>(type: "longtext", nullable: false, collation: "utf8_general_ci"),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8_general_ci"),
                    ProviderKey = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8_general_ci"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8_general_ci"),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8_general_ci"),
                    Value = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "Beneficiari",
                columns: table => new
                {
                    IdBeneficiario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Beneficiario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(3000)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SitoWeb = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IdUser = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiari", x => x.IdBeneficiario);
                    table.ForeignKey(
                        name: "FK_Beneficiari_AspNetUsers_IdUser",
                        column: x => x.IdUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "Scadenze",
                columns: table => new
                {
                    IDScadenza = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IDUser = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci"),
                    IDBeneficiario = table.Column<int>(type: "int", nullable: false),
                    Beneficiario = table.Column<string>(type: "nvarchar(100)", maxLength: 150, nullable: false),
                    DataScadenza = table.Column<DateTime>(type: "datetime", nullable: false),
                    Importo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Sollecito = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 0ul),
                    GiorniRitardo = table.Column<short>(type: "smallint", nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scadenze", x => x.IDScadenza);
                    table.ForeignKey(
                        name: "FK_Scadenze_Beneficiario",
                        column: x => x.IDBeneficiario,
                        principalTable: "Beneficiari",
                        principalColumn: "IdBeneficiario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "Ricevute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdScadenza = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FileContent = table.Column<byte[]>(type: "LONGBLOB", nullable: false),
                    Beneficiario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ricevute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scadenze_Ricevute",
                        column: x => x.IdScadenza,
                        principalTable: "Scadenze",
                        principalColumn: "IDScadenza",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ScadenzaId = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    PaymentType = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscription_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_Scadenze_ScadenzaId",
                        column: x => x.ScadenzaId,
                        principalTable: "Scadenze",
                        principalColumn: "IDScadenza",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiari_IdUser",
                table: "Beneficiari",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Ricevute_IdScadenza",
                table: "Ricevute",
                column: "IdScadenza");

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_IDBeneficiario",
                table: "Scadenze",
                column: "IDBeneficiario");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_ScadenzaId",
                table: "Subscription",
                column: "ScadenzaId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_UserId",
                table: "Subscription",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Ricevute");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Scadenze");

            migrationBuilder.DropTable(
                name: "Beneficiari");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
