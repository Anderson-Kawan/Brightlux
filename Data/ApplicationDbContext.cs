using Brightlux.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Brightlux.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(entity =>
        {
            // Mantém compatibilidade com o banco criado antes da padronização em inglês.
            entity.ToTable("Produtos");
            entity.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.Categoria)
                .HasMaxLength(100);

            entity.Property(p => p.Descricao)
                .HasMaxLength(500);

            entity.Property(p => p.Imagem)
                .HasMaxLength(500);

            entity.Property(p => p.Aplicacao)
                .HasMaxLength(100);
        });

        builder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("ProdutoImagens");
            entity.Property(i => i.Url).IsRequired().HasMaxLength(500);
            entity.HasOne(i => i.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
