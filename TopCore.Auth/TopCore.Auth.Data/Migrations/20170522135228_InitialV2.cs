using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TopCore.Auth.Data.Migrations
{
    public partial class InitialV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastUpdatedOnUtc",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedOnUtc",
                schema: "dbo",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedTime",
                schema: "dbo",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedTime",
                schema: "dbo",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GlobalId",
                schema: "dbo",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedTime",
                schema: "dbo",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeletedTime",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GlobalId",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastUpdatedTime",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedOnUtc",
                schema: "dbo",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                schema: "dbo",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOnUtc",
                schema: "dbo",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
