namespace WealthOS.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.DTOs;
using WealthOS.Application.Interfaces;
using WealthOS.Web.Models;
using WealthOS.Web.Services;

public class SavingController : Controller
{
    private readonly IFinanceService _financeService;
    private readonly ICurrentUserContext _currentUser;

    public SavingController(IFinanceService financeService, ICurrentUserContext currentUser)
    {
        _financeService = financeService;
        _currentUser = currentUser;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);
        var vm = new SavingFormViewModel { Existing = await _financeService.GetSavingsAsync(userId, ct) };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SavingFormViewModel vm, CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);

        if (!ModelState.IsValid)
        {
            vm.Existing = await _financeService.GetSavingsAsync(userId, ct);
            return View("Index", vm);
        }

        await _financeService.AddSavingAsync(userId, new CreateSavingDto(vm.Amount, vm.Date, vm.Note), ct);
        return RedirectToAction(nameof(Index));
    }
}
