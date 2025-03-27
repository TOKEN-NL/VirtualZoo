using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualZooAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddZooIdToEnclosure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enclosures_Zoos_ZooId",
                table: "Enclosures");

            migrationBuilder.AlterColumn<int>(
                name: "ZooId",
                table: "Enclosures",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enclosures_Zoos_ZooId",
                table: "Enclosures",
                column: "ZooId",
                principalTable: "Zoos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enclosures_Zoos_ZooId",
                table: "Enclosures");

            migrationBuilder.AlterColumn<int>(
                name: "ZooId",
                table: "Enclosures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Enclosures_Zoos_ZooId",
                table: "Enclosures",
                column: "ZooId",
                principalTable: "Zoos",
                principalColumn: "Id");
        }
    }
}
