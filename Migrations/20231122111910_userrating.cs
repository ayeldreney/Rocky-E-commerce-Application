using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rocky.Migrations
{
	/// <inheritdoc />
	public partial class userrating : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "ProductId",
				table: "UserReviews",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateIndex(
				name: "IX_UserReviews_ProductId",
				table: "UserReviews",
				column: "ProductId");

			migrationBuilder.AddForeignKey(
				name: "FK_UserReviews_Products_ProductId",
				table: "UserReviews",
				column: "ProductId",
				principalTable: "Products",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_UserReviews_Products_ProductId",
				table: "UserReviews");

			migrationBuilder.DropIndex(
				name: "IX_UserReviews_ProductId",
				table: "UserReviews");

			migrationBuilder.DropColumn(
				name: "ProductId",
				table: "UserReviews");
		}
	}
}
