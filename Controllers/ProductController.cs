using Brightlux.Data;
using Brightlux.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brightlux.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Where(p => p.Ativo)
            .OrderBy(p => p.Ordem)
            .ThenBy(p => p.Nome)
            .ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Images.OrderBy(i => i.Ordem))
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Admin()
    {
        var products = await _context.Products
            .OrderBy(p => p.Ordem)
            .ThenBy(p => p.Nome)
            .ToListAsync();

        return View(products);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new Product { Ativo = true });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        var imageUrls = ValidateAndParseImages(product.Imagem, product.AdditionalImagesInput);
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        product.Id = 0;
        product.DataCadastro = DateTime.UtcNow;
        Normalize(product);
        SetImages(product, imageUrls);

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Produto cadastrado com sucesso.";
        return RedirectToAction(nameof(Admin));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products
            .Include(p => p.Images.OrderBy(i => i.Ordem))
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) return NotFound();

        product.AdditionalImagesInput = string.Join(Environment.NewLine, product.Images.Select(i => i.Url));
        return View(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        var imageUrls = ValidateAndParseImages(product.Imagem, product.AdditionalImagesInput);
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var existing = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Nome = product.Nome;
        existing.Categoria = product.Categoria;
        existing.Descricao = product.Descricao;
        existing.Imagem = product.Imagem;
        existing.Aplicacao = product.Aplicacao;
        existing.Ativo = product.Ativo;
        existing.Destaque = product.Destaque;
        existing.Ordem = product.Ordem;
        Normalize(existing);
        _context.ProductImages.RemoveRange(existing.Images);
        SetImages(existing, imageUrls);

        await _context.SaveChangesAsync();
        TempData["Success"] = "Produto atualizado com sucesso.";
        return RedirectToAction(nameof(Admin));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Produto excluído com sucesso.";
        return RedirectToAction(nameof(Admin));
    }

    private static void Normalize(Product product)
    {
        product.Nome = product.Nome.Trim();
        product.Categoria = product.Categoria.Trim();
        product.Descricao = product.Descricao.Trim();
        product.Imagem = product.Imagem.Trim();
        product.Aplicacao = product.Aplicacao.Trim();
    }

    private List<string> ValidateAndParseImages(string? coverImage, string? input)
    {
        coverImage = coverImage?.Trim();

        if (!string.IsNullOrEmpty(coverImage) &&
            (coverImage.Length > 500 || !IsSafeImageLocation(coverImage)))
        {
            ModelState.AddModelError(nameof(Product.Imagem),
                "Use um caminho iniciado por / ou uma URL HTTPS válida.");
        }

        var urls = (input ?? string.Empty)
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (urls.Count > 10)
        {
            ModelState.AddModelError(nameof(Product.AdditionalImagesInput), "Informe no máximo 10 imagens adicionais.");
        }

        if (urls.Any(url => url.Length > 500 || !IsSafeImageLocation(url)))
        {
            ModelState.AddModelError(nameof(Product.AdditionalImagesInput),
                "Use somente caminhos iniciados por / ou URLs HTTPS válidas, uma por linha.");
        }

        return urls;
    }

    private static bool IsSafeImageLocation(string value) =>
        value.StartsWith('/') ||
        (Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps);

    private static void SetImages(Product product, IEnumerable<string> urls)
    {
        product.Images = urls.Select((url, index) => new ProductImage
        {
            Url = url,
            Ordem = index + 1
        }).ToList();
    }
}
