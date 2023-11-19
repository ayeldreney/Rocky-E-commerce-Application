using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rocky.Migrations
{
	/// <inheritdoc />
	public partial class shortDescriptionColumnAdded : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "Product",
				type: "nvarchar(500)",
				maxLength: 500,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)");

			migrationBuilder.AlterColumn<string>(
				name: "Description",
				table: "Product",
				type: "nvarchar(1000)",
				maxLength: 1000,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)");

			migrationBuilder.AddColumn<string>(
				name: "ShortDesc",
				table: "Product",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "ShortDesc",
				table: "Product");

			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "Product",
				type: "nvarchar(max)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(500)",
				oldMaxLength: 500);

			migrationBuilder.AlterColumn<string>(
				name: "Description",
				table: "Product",
				type: "nvarchar(max)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(1000)",
				oldMaxLength: 1000);
		}
	}
}
