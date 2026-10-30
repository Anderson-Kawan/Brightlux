using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brightlux.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do produto.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "A categoria deve ter no máximo 100 caracteres.")]
    [Display(Name = "Categoria")]
    public string Categoria { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "O caminho da imagem deve ter no máximo 500 caracteres.")]
    [Display(Name = "Imagem (URL ou caminho)")]
    public string Imagem { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "A aplicação deve ter no máximo 100 caracteres.")]
    [Display(Name = "Aplicação")]
    public string Aplicacao { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public bool Destaque { get; set; } = false;

    [Range(0, int.MaxValue, ErrorMessage = "A ordem não pode ser negativa.")]
    [Display(Name = "Ordem de exibição")]
    public int Ordem { get; set; } = 0;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    [NotMapped]
    [Display(Name = "Imagens adicionais")]
    public string AdditionalImagesInput { get; set; } = string.Empty;
}
