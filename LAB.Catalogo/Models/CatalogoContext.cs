using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LAB.Catalogo.Models
{
    public class CatalogoContext : DbContext
    {
        public CatalogoContext(DbContextOptions<CatalogoContext> options)
            : base(options) { }

        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().HasData(Seed.Produtos());
            base.OnModelCreating(modelBuilder);
        }
    }

    public class Seed
    {
        public static IEnumerable<Produto> Produtos() => new Produto[]{
          new Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Camiseta Guns n'Roses",
                Descricao = "Tamanhos Disponíveis: P,M,G,GG",
                Preco = 45.78M,
                Categoria = "Roupas",
                Imagem = "https://img.elo7.com.br/product/original/13EC3B1/camiseta-guns-n-roses-camisetas-personalizadas-de-aniversario.jpg"
            },
          new Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Camiseta Metálica",
                Descricao = "Tamanhos Disponíveis: P,M,G,GG",
                Preco = 43.78M,
                Categoria = "Roupas",
                Imagem = "https://static.stamp.com.br/produto/TS1446_0_TS1446_0.jpeg"
            },
          new Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Camiseta BON JOVI [Feminina]",
                Descricao = "Tamanhos Disponíveis: P,M,G,GG",
                Preco = 38.23M,
                Categoria = "Roupas",
                Imagem = "https://img.elo7.com.br/product/original/1A8045E/camiseta-bon-jovi-separe-suas-tags-com-virgulas.jpg"
            },
          new Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Camiseta Iron Maden",
                Descricao = "Tamanhos Disponíveis: P,M,G,GG",
                Preco = 55.99M,
                Categoria = "Roupas",
                Imagem = "https://m.media-amazon.com/images/I/6164EADBuNL._AC_SX385_.jpg"
            },
        };
    }
}