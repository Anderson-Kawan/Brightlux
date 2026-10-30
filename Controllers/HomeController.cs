using Brightlux.Data;
using Brightlux.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brightlux.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Where(p => p.Ativo && p.Destaque)
            .OrderBy(p => p.Ordem)
            .ThenByDescending(p => p.DataCadastro)
            .Take(4)
            .ToListAsync();

        return View(products);
    }
}
