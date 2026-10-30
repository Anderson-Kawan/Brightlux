using Brightlux.Data;
using Brightlux.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Brightlux.Controllers;

public class AdminController : Controller
{
    private const string AdminRole = "Admin";
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public AdminController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _environment = environment;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true && User.IsInRole(AdminRole))
        {
            return RedirectToAction("Admin", "Product");
        }

        if (!await AdminExistsAsync())
        {
            return RedirectToAction(nameof(Setup));
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new AdminLoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email.Trim());
        if (user is null || !await _userManager.IsInRoleAsync(user, AdminRole))
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty,
                result.IsLockedOut
                    ? "Acesso temporariamente bloqueado. Tente novamente mais tarde."
                    : "E-mail ou senha inválidos.");
            return View(model);
        }

        return Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction("Admin", "Product");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Setup()
    {
        if (await AdminExistsAsync())
        {
            return RedirectToAction(nameof(Login));
        }

        if (!SetupIsAvailable()) return NotFound();
        ViewData["RequiresSetupKey"] = RequiresSetupKey();
        return View(new AdminSetupViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Setup(AdminSetupViewModel model)
    {
        if (await AdminExistsAsync())
        {
            return RedirectToAction(nameof(Login));
        }

        if (!SetupIsAvailable()) return NotFound();
        ViewData["RequiresSetupKey"] = RequiresSetupKey();

        if (!SetupKeyIsValid(model.SetupKey))
        {
            ModelState.AddModelError(nameof(model.SetupKey), "Chave de instalação inválida.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!await _roleManager.RoleExistsAsync(AdminRole))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(AdminRole));
            if (!roleResult.Succeeded)
            {
                AddErrors(roleResult);
                return View(model);
            }
        }

        var user = new IdentityUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
        {
            AddErrors(createResult);
            return View(model);
        }

        var roleAssignment = await _userManager.AddToRoleAsync(user, AdminRole);
        if (!roleAssignment.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            AddErrors(roleAssignment);
            return View(model);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Admin", "Product");
    }

    [HttpPost]
    [Authorize(Roles = AdminRole)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    private Task<bool> AdminExistsAsync() =>
        _context.UserRoles
            .Join(_context.Roles.Where(r => r.Name == AdminRole),
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => userRole)
            .AnyAsync();

    private bool RequiresSetupKey() =>
        !string.IsNullOrWhiteSpace(_configuration["AdminSetup:Key"]);

    private bool SetupIsAvailable() => RequiresSetupKey() ||
        (_environment.IsDevelopment() && HttpContext.Connection.RemoteIpAddress is { } ip &&
         System.Net.IPAddress.IsLoopback(ip));

    private bool SetupKeyIsValid(string suppliedKey)
    {
        var configuredKey = _configuration["AdminSetup:Key"];
        if (string.IsNullOrWhiteSpace(configuredKey)) return SetupIsAvailable();

        return CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(configuredKey)),
            SHA256.HashData(Encoding.UTF8.GetBytes(suppliedKey ?? string.Empty)));
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, TranslateIdentityError(error.Code));
        }
    }

    private static string TranslateIdentityError(string code) => code switch
    {
        "DuplicateEmail" or "DuplicateUserName" => "Este e-mail já está cadastrado.",
        "PasswordRequiresDigit" => "A senha precisa conter um número.",
        "PasswordRequiresLower" => "A senha precisa conter uma letra minúscula.",
        "PasswordRequiresUpper" => "A senha precisa conter uma letra maiúscula.",
        "PasswordTooShort" => "A senha precisa ter pelo menos 8 caracteres.",
        _ => "Não foi possível criar o acesso administrativo. Confira os dados."
    };
}
