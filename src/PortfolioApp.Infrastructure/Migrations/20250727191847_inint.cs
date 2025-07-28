using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class inint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false, comment: "Unique identifier for the contact message")
                        .Annotation("Sqlite:Autoincrement", true),
                    SenderName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Name of the person sending the message"),
                    SenderEmail = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false, comment: "Email address of the sender"),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Subject line of the message"),
                    Message = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false, comment: "The actual message content from the sender"),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')", comment: "Timestamp when the message was sent/received"),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Indicates whether the message has been read by an administrator")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                    table.CheckConstraint("CK_ContactMessages_Message_Length", "LENGTH(TRIM(Message)) >= 10");
                    table.CheckConstraint("CK_ContactMessages_Message_NotEmpty", "LENGTH(TRIM(Message)) > 0");
                    table.CheckConstraint("CK_ContactMessages_SenderEmail_Format", "SenderEmail LIKE '%@%.%' AND LENGTH(SenderEmail) > 5");
                    table.CheckConstraint("CK_ContactMessages_SenderEmail_NotTest", "SenderEmail NOT LIKE '%test@test%' AND SenderEmail NOT LIKE '%example.com%'");
                    table.CheckConstraint("CK_ContactMessages_SenderName_Length", "LENGTH(TRIM(SenderName)) >= 2");
                    table.CheckConstraint("CK_ContactMessages_SenderName_NotEmpty", "LENGTH(TRIM(SenderName)) > 0");
                    table.CheckConstraint("CK_ContactMessages_SentAt_Valid", "SentAt <= datetime('now')");
                    table.CheckConstraint("CK_ContactMessages_Subject_Length", "LENGTH(TRIM(Subject)) >= 3");
                    table.CheckConstraint("CK_ContactMessages_Subject_NotEmpty", "LENGTH(TRIM(Subject)) > 0");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false, comment: "Unique identifier for the user")
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "User's first name"),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "User's last name"),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false, comment: "User's email address"),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true, comment: "User's phone number"),
                    Bio = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false, defaultValue: "", comment: "User's professional biography"),
                    ProfilePictureUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, comment: "Relative URL to user's profile picture"),
                    ResumeUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, comment: "Relative URL to user's resume file"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')", comment: "Timestamp when the user record was created"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')", comment: "Timestamp when the user record was last updated")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CK_Users_CreatedAt_Valid", "CreatedAt <= datetime('now')");
                    table.CheckConstraint("CK_Users_Email_Format", "Email LIKE '%@%.%' AND LENGTH(Email) > 5");
                    table.CheckConstraint("CK_Users_FirstName_NotEmpty", "LENGTH(TRIM(FirstName)) > 0");
                    table.CheckConstraint("CK_Users_LastName_NotEmpty", "LENGTH(TRIM(LastName)) > 0");
                    table.CheckConstraint("CK_Users_PhoneNumber_Format", "PhoneNumber IS NULL OR LENGTH(TRIM(PhoneNumber)) >= 10");
                    table.CheckConstraint("CK_Users_UpdatedAt_Valid", "UpdatedAt >= CreatedAt");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false, comment: "Unique identifier for the project")
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false, comment: "Foreign key reference to the user who owns this project"),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Project title"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false, comment: "Detailed project description"),
                    TechnologiesUsed = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false, defaultValue: "", comment: "Comma-separated list of technologies used"),
                    ProjectUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, comment: "URL to the live project or repository"),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "Project start date"),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "Project end date (null for ongoing projects)"),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Indicates if project should be featured prominently"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')", comment: "Timestamp when the project record was created"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')", comment: "Timestamp when the project record was last updated")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.CheckConstraint("CK_Projects_CreatedAt_Valid", "CreatedAt <= datetime('now')");
                    table.CheckConstraint("CK_Projects_DateRange_Valid", "EndDate IS NULL OR StartDate IS NULL OR EndDate >= StartDate");
                    table.CheckConstraint("CK_Projects_Description_NotEmpty", "LENGTH(TRIM(Description)) > 0");
                    table.CheckConstraint("CK_Projects_ProjectUrl_Format", "ProjectUrl IS NULL OR ProjectUrl LIKE 'http%://%'");
                    table.CheckConstraint("CK_Projects_Title_NotEmpty", "LENGTH(TRIM(Title)) > 0");
                    table.CheckConstraint("CK_Projects_UpdatedAt_Valid", "UpdatedAt >= CreatedAt");
                    table.CheckConstraint("CK_Projects_UserId_Positive", "UserId > 0");
                    table.ForeignKey(
                        name: "FK_Projects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false, comment: "Unique identifier for the image")
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false, comment: "Foreign key reference to the project this image belongs to"),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Display title for the image"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false, defaultValue: "", comment: "Detailed description of the image content"),
                    Url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Relative URL path to the image file from wwwroot"),
                    AltText = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, defaultValue: "", comment: "Alternative text for accessibility (screen readers)"),
                    Width = table.Column<int>(type: "INTEGER", nullable: false, comment: "Image width in pixels"),
                    Height = table.Column<int>(type: "INTEGER", nullable: false, comment: "Image height in pixels"),
                    SizeInBytes = table.Column<long>(type: "INTEGER", nullable: false, comment: "File size in bytes"),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')", comment: "Timestamp when the image was uploaded")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.CheckConstraint("CK_Images_Height_Positive", "Height > 0");
                    table.CheckConstraint("CK_Images_Height_Reasonable", "Height <= 10000");
                    table.CheckConstraint("CK_Images_ProjectId_Positive", "ProjectId > 0");
                    table.CheckConstraint("CK_Images_SizeInBytes_Positive", "SizeInBytes > 0");
                    table.CheckConstraint("CK_Images_SizeInBytes_Reasonable", "SizeInBytes <= 52428800");
                    table.CheckConstraint("CK_Images_Title_NotEmpty", "LENGTH(TRIM(Title)) > 0");
                    table.CheckConstraint("CK_Images_UploadedAt_Valid", "UploadedAt <= datetime('now')");
                    table.CheckConstraint("CK_Images_Url_Format", "Url LIKE '/%' OR Url LIKE 'http%://%'");
                    table.CheckConstraint("CK_Images_Url_NotEmpty", "LENGTH(TRIM(Url)) > 0");
                    table.CheckConstraint("CK_Images_Width_Positive", "Width > 0");
                    table.CheckConstraint("CK_Images_Width_Reasonable", "Width <= 10000");
                    table.ForeignKey(
                        name: "FK_Images_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false, comment: "Unique identifier for the video")
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false, comment: "Foreign key reference to the project this video belongs to"),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Display title for the video"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false, defaultValue: "", comment: "Detailed description of the video content"),
                    Url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Relative URL path to the video file from wwwroot"),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, comment: "Relative URL path to the video thumbnail image"),
                    DurationInSeconds = table.Column<int>(type: "INTEGER", nullable: false, comment: "Video duration in seconds"),
                    SizeInBytes = table.Column<long>(type: "INTEGER", nullable: false, comment: "File size in bytes"),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')", comment: "Timestamp when the video was uploaded")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.CheckConstraint("CK_Videos_DurationInSeconds_NonNegative", "DurationInSeconds >= 0");
                    table.CheckConstraint("CK_Videos_DurationInSeconds_Reasonable", "DurationInSeconds <= 14400");
                    table.CheckConstraint("CK_Videos_ProjectId_Positive", "ProjectId > 0");
                    table.CheckConstraint("CK_Videos_SizeInBytes_Positive", "SizeInBytes > 0");
                    table.CheckConstraint("CK_Videos_SizeInBytes_Reasonable", "SizeInBytes <= 2147483648");
                    table.CheckConstraint("CK_Videos_ThumbnailUrl_Format", "ThumbnailUrl IS NULL OR ThumbnailUrl LIKE '/%' OR ThumbnailUrl LIKE 'http%://%'");
                    table.CheckConstraint("CK_Videos_Title_NotEmpty", "LENGTH(TRIM(Title)) > 0");
                    table.CheckConstraint("CK_Videos_UploadedAt_Valid", "UploadedAt <= datetime('now')");
                    table.CheckConstraint("CK_Videos_Url_Format", "Url LIKE '/%' OR Url LIKE 'http%://%'");
                    table.CheckConstraint("CK_Videos_Url_NotEmpty", "LENGTH(TRIM(Url)) > 0");
                    table.ForeignKey(
                        name: "FK_Videos_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_IsRead",
                table: "ContactMessages",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_SenderEmail",
                table: "ContactMessages",
                column: "SenderEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_SenderEmail_SentAt",
                table: "ContactMessages",
                columns: new[] { "SenderEmail", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_SenderName",
                table: "ContactMessages",
                column: "SenderName");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_SentAt",
                table: "ContactMessages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_Subject",
                table: "ContactMessages",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_Urgent",
                table: "ContactMessages",
                columns: new[] { "IsRead", "SentAt" },
                filter: "IsRead = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Dimensions",
                table: "Images",
                columns: new[] { "Width", "Height" });

            migrationBuilder.CreateIndex(
                name: "IX_Images_ProjectId",
                table: "Images",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ProjectId_UploadedAt",
                table: "Images",
                columns: new[] { "ProjectId", "UploadedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Images_SizeInBytes",
                table: "Images",
                column: "SizeInBytes");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Title",
                table: "Images",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Images_UploadedAt",
                table: "Images",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Url_Unique",
                table: "Images",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedAt",
                table: "Projects",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DateRange",
                table: "Projects",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_EndDate",
                table: "Projects",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsFeatured",
                table: "Projects",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_StartDate",
                table: "Projects",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Title",
                table: "Projects",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId_IsFeatured",
                table: "Projects",
                columns: new[] { "UserId", "IsFeatured" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId_Title_Unique",
                table: "Projects",
                columns: new[] { "UserId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_DurationInSeconds",
                table: "Videos",
                column: "DurationInSeconds");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ProjectId",
                table: "Videos",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ProjectId_UploadedAt",
                table: "Videos",
                columns: new[] { "ProjectId", "UploadedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_SizeInBytes",
                table: "Videos",
                column: "SizeInBytes");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ThumbnailUrl",
                table: "Videos",
                column: "ThumbnailUrl");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_Title",
                table: "Videos",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_UploadedAt",
                table: "Videos",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_Url_Unique",
                table: "Videos",
                column: "Url",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
