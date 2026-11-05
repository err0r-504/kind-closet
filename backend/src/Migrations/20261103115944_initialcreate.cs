using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace kind_closet.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DefaultWeightKg = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: false),
                    Co2PerKgFactor = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IsEmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    IsPhoneConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    OAuthProvider = table.Column<int>(type: "integer", nullable: false),
                    OAuthId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AvatarObjectKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    PointsBalance = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    SavedKgTotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    SavedCo2Total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    WorkingHoursJson = table.Column<string>(type: "jsonb", nullable: false),
                    ContactsJson = table.Column<string>(type: "jsonb", nullable: false),
                    SupportedFormats = table.Column<int[]>(type: "integer[]", nullable: false),
                    AcceptedCategoryIds = table.Column<int[]>(type: "integer[]", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsPartner = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangePoints_Users_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Listings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    ExchangePointId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    PostType = table.Column<int>(type: "integer", nullable: false),
                    Condition = table.Column<int>(type: "integer", nullable: false),
                    Dimensions = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExchangeFormat = table.Column<int>(type: "integer", nullable: false),
                    PointsPrice = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ViewsCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listings_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Listings_ExchangePoints_ExchangePointId",
                        column: x => x.ExchangePointId,
                        principalTable: "ExchangePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Listings_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListingImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListingId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalObjectKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ThumbnailObjectKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListingImages_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListingId = table.Column<Guid>(type: "uuid", nullable: false),
                    GiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExchangePointId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GiverConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    ReceiverConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    AwardedPoints = table.Column<int>(type: "integer", nullable: false),
                    CalculatedWeightKg = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: false),
                    CalculatedCo2Kg = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfers_ExchangePoints_ExchangePointId",
                        column: x => x.ExchangePointId,
                        principalTable: "ExchangePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transfers_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_Users_GiverId",
                        column: x => x.GiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangePoints_City_Status",
                table: "ExchangePoints",
                columns: new[] { "City", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangePoints_OrganizerId",
                table: "ExchangePoints",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingImages_ListingId",
                table: "ListingImages",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_AuthorId",
                table: "Listings",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_CategoryId",
                table: "Listings",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_CreatedAt",
                table: "Listings",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_ExchangePointId",
                table: "Listings",
                column: "ExchangePointId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_Status_City_CategoryId_ExchangeFormat",
                table: "Listings",
                columns: new[] { "Status", "City", "CategoryId", "ExchangeFormat" });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ExchangePointId",
                table: "Transfers",
                column: "ExchangePointId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_GiverId",
                table: "Transfers",
                column: "GiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ListingId_Status",
                table: "Transfers",
                columns: new[] { "ListingId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ReceiverId",
                table: "Transfers",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_OAuthProvider_OAuthId",
                table: "Users",
                columns: new[] { "OAuthProvider", "OAuthId" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListingImages");

            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "Listings");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "ExchangePoints");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
