﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BultyBook.DataAccess.Migrations
{
    public partial class addTrackingNumberToOrderHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "OrderHeaders");
        }
    }
}
