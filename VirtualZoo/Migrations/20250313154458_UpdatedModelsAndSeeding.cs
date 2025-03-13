using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualZoo.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModelsAndSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Climate",
                table: "Enclosures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HabitatType",
                table: "Enclosures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecurityLevel",
                table: "Enclosures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Size",
                table: "Enclosures",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ActivityPattern",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DietaryClass",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Prey",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SecurityRequirement",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SpaceRequirement",
                table: "Animals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Species",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Climate",
                table: "Enclosures");

            migrationBuilder.DropColumn(
                name: "HabitatType",
                table: "Enclosures");

            migrationBuilder.DropColumn(
                name: "SecurityLevel",
                table: "Enclosures");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Enclosures");

            migrationBuilder.DropColumn(
                name: "ActivityPattern",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "DietaryClass",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Prey",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "SecurityRequirement",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "SpaceRequirement",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Species",
                table: "Animals");
        }
    }
}
