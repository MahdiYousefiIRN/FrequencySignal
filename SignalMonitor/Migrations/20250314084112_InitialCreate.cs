using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignalMonitor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PacketData",
                columns: table => new
                {
                    PacketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceIP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationIP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Protocol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    PreviousValue = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMalicious = table.Column<bool>(type: "bit", nullable: false),
                    SecurityKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacketData", x => x.PacketId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PacketData");
        }
    }
}
