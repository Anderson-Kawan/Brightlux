using System.ComponentModel.DataAnnotations;

namespace Brightlux.Models;

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    [Required, StringLength(500)]
    public string Url { get; set; } = string.Empty;

    public int Ordem { get; set; }
    public Product Product { get; set; } = null!;
}
