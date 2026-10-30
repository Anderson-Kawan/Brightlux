using Brightlux.Models;
using Microsoft.EntityFrameworkCore;

namespace Brightlux.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Products.AnyAsync())
        {
            return;
        }

        var products = new List<Product>
        {
            new Product
            {
                Nome = "High Bay LED",
                Categoria = "Industrial",
                Descricao = "Luminária LED de alta performance desenvolvida para grandes ambientes industriais.",
                Imagem = "/images/produtos/high-bay.jpg",
                Aplicacao = "Indústrias e galpões",
                Ativo = true,
                Destaque = true,
                Ordem = 1
            },

            new Product
            {
                Nome = "Luminária Pública LED",
                Categoria = "Iluminação Pública",
                Descricao = "Solução LED para iluminação pública com alta eficiência e durabilidade.",
                Imagem = "/images/produtos/publica.jpg",
                Aplicacao = "Vias públicas",
                Ativo = true,
                Destaque = true,
                Ordem = 2
            },

            new Product
            {
                Nome = "Projetor LED",
                Categoria = "Externo",
                Descricao = "Projetor LED de alta potência para áreas externas e grandes espaços.",
                Imagem = "/images/produtos/projetor.jpg",
                Aplicacao = "Áreas externas",
                Ativo = true,
                Destaque = true,
                Ordem = 3
            },

            new Product
            {
                Nome = "Solução Linear LED",
                Categoria = "Comercial",
                Descricao = "Sistema de iluminação linear desenvolvido para ambientes comerciais e corporativos.",
                Imagem = "/images/produtos/linear.jpg",
                Aplicacao = "Comercial e corporativo",
                Ativo = true,
                Destaque = true,
                Ordem = 4
            }
        };

        await context.Products.AddRangeAsync(products);

        await context.SaveChangesAsync();
    }
}
