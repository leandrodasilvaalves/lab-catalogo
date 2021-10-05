using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LAB.Catalogo.Migrations
{
    public partial class Initial_database : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: true),
                    Imagem = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Categoria", "Descricao", "Imagem", "Nome", "Preco" },
                values: new object[,]
                {
                    { new Guid("96613c80-a99e-4ecb-8504-1a1001e2f86e"), "Roupas", "Tamanhos Disponíveis: P,M,G,GG", "https://img.elo7.com.br/product/original/13EC3B1/camiseta-guns-n-roses-camisetas-personalizadas-de-aniversario.jpg", "Camiseta Guns n'Roses", 45.78m },
                    { new Guid("782ecd42-1a82-44cb-9264-12f881ca31c2"), "Roupas", "Tamanhos Disponíveis: P,M,G,GG", "https://static.stamp.com.br/produto/TS1446_0_TS1446_0.jpeg", "Camiseta Metálica", 43.78m },
                    { new Guid("1ccca33a-28eb-474c-949f-96d7b5a3475b"), "Roupas", "Tamanhos Disponíveis: P,M,G,GG", "https://img.elo7.com.br/product/original/1A8045E/camiseta-bon-jovi-separe-suas-tags-com-virgulas.jpg", "Camiseta BON JOVI [Feminina]", 38.23m },
                    { new Guid("0655cfd4-d2fc-4c8b-bd0d-15afec8f0277"), "Roupas", "Tamanhos Disponíveis: P,M,G,GG", "https://m.media-amazon.com/images/I/6164EADBuNL._AC_SX385_.jpg", "Camiseta Iron Maden", 55.99m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produtos");
        }
    }
}
