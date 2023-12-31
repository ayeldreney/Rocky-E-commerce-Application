using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rocky.Migrations
{
	/// <inheritdoc />
	public partial class final : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Unit",
				table: "Products",
				type: "nvarchar(max)",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Vendor",
				table: "Products",
				type: "nvarchar(max)",
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Unit",
				table: "Products");

			migrationBuilder.DropColumn(
				name: "Vendor",
				table: "Products");
		}
	}
}
