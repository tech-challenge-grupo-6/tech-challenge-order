using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControladorPedidos.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CategoriaProduto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaProduto", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Cpf = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CategoriaId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Preco = table.Column<double>(type: "double", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    Imagem = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produto_CategoriaProduto_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "CategoriaProduto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ClienteId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<double>(type: "double", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pedido_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PedidoProduto",
                columns: table => new
                {
                    PedidosId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProdutosId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoProduto", x => new { x.PedidosId, x.ProdutosId });
                    table.ForeignKey(
                        name: "FK_PedidoProduto_Pedido_PedidosId",
                        column: x => x.PedidosId,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoProduto_Produto_ProdutosId",
                        column: x => x.ProdutosId,
                        principalTable: "Produto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_ClienteId",
                table: "Pedido",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoProduto_ProdutosId",
                table: "PedidoProduto",
                column: "ProdutosId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_CategoriaId",
                table: "Produto",
                column: "CategoriaId");

            Seed(migrationBuilder);
        }

        private static void Seed(MigrationBuilder migrationBuilder)
        {
            // Cliente
            migrationBuilder.InsertData(
                table: "Cliente",
                columns: ["Id", "Nome", "Email", "Cpf", "CriadoEm", "AtualizadoEm"],
                values: new object[,]
                {
                    {new Guid("5f7b8c70-c799-4d31-a9ff-a5f013a9f488"), "John Doe", "johndoe@email.com", "35042084061", DateTime.Now, DateTime.Now}
                });
            // Categoria Produto
            var LancheId = new Guid("d2e99fdb-2aba-4f97-809e-8b884d41962b");
            var BebidaId = new Guid("13bbe72f-2ef0-4763-aee0-14d26177907d");
            var ComplementoId = new Guid("960b7176-3a86-42ae-addb-ba0aeea59a7e");
            migrationBuilder.InsertData(
                table: "CategoriaProduto",
                columns: ["Id", "Nome", "CriadoEm", "AtualizadoEm"],
                values: new object[,]
                {
                    {LancheId, "Lanche", DateTime.Now, DateTime.Now}
                });
            migrationBuilder.InsertData(
                table: "CategoriaProduto",
                columns: ["Id", "Nome", "CriadoEm", "AtualizadoEm"],
                values: new object[,]
                {
                    {BebidaId, "Bebida", DateTime.Now, DateTime.Now}
                });
            migrationBuilder.InsertData(
                table: "CategoriaProduto",
                columns: ["Id", "Nome", "CriadoEm", "AtualizadoEm"],
                values: new object[,]
                {
                    {ComplementoId, "Complemento", DateTime.Now, DateTime.Now}
                });
            // Produtos
            migrationBuilder.InsertData(
                table: "Produto",
                columns: ["Id", "Nome", "CategoriaId", "Preco", "Descricao", "Imagem", "CriadoEm", "AtualizadoEm"],
                values: new object[,]
                {
                    {new Guid("9577e2e4-a4a0-4caa-a0a1-5e7df5560b00"), "Coca-cola", BebidaId, 10, "Coca-cola", "coca-cola.png", DateTime.Now, DateTime.Now}
                });
            migrationBuilder.InsertData(
                table: "Produto",
                columns: ["Id", "Nome", "CategoriaId", "Preco", "Descricao", "Imagem", "CriadoEm", "AtualizadoEm"],
                values: new object[,]
                {
                    {new Guid("d8dac810-ea8e-43cf-aace-b5a600f8d500"), "Pepsi", BebidaId, 10, "Pepsi", "pepsi.png", DateTime.Now, DateTime.Now}
                });
            migrationBuilder.InsertData(
                table: "Produto",
                columns: ["Id", "Nome", "CategoriaId", "Preco", "Descricao", "Imagem", "CriadoEm", "AtualizadoEm"],
                values: new object[,]
                {
                    {new Guid("8d3af3e6-cee2-4676-8c62-78059201108d"), "Hámburguer", LancheId, 35, "Hámburguer", "hamburguer.png", DateTime.Now, DateTime.Now}
                });
            migrationBuilder.InsertData(
                table: "Produto",
                columns: ["Id", "Nome", "CategoriaId", "Preco", "Descricao", "Imagem", "CriadoEm", "AtualizadoEm"],
                values: new object[,]
                {
                    {new Guid("6c00ca73-3bb9-4c29-9faf-a5e91735b06e"), "Pizza", LancheId, 50, "Pizza", "pizza.png", DateTime.Now, DateTime.Now}
                });
            migrationBuilder.InsertData(
                table: "Produto",
                columns: ["Id", "Nome", "CategoriaId", "Preco", "Descricao", "Imagem", "CriadoEm", "AtualizadoEm"],
                values: new object[,]
                {
                    {new Guid("c1853b41-33ce-49e7-bcfb-5a4c111f32d7"), "Bacon", ComplementoId, 1, "Bacon", "bacon.png", DateTime.Now, DateTime.Now}
                });
            migrationBuilder.InsertData(
                table: "Produto",
                columns: ["Id", "Nome", "CategoriaId", "Preco", "Descricao", "Imagem", "CriadoEm", "AtualizadoEm"],
                values: new object[,]
                {
                    {new Guid("59fc02f4-801f-415b-bdce-3cd8060c6c9a"), "Ketchup", ComplementoId, 1, "Ketchup", "ketchup.png", DateTime.Now, DateTime.Now}
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidoProduto");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "CategoriaProduto");
        }
    }
}
