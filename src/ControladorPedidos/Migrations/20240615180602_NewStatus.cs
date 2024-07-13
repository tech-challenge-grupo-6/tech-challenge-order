using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControladorPedidos.Migrations
{
    /// <inheritdoc />
    public partial class NewStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Excluido",
                table: "Pedido",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Pago",
                table: "Pedido",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Excluido",
                table: "Cliente",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Excluido",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Pago",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Excluido",
                table: "Cliente");
        }
    }
}
