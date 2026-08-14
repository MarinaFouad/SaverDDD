namespace WealthOS.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.DTOs;
using WealthOS.Application.Interfaces;
using WealthOS.Web.Models;
using WealthOS.Web.Services;

public class ExpenseController : Controller
{
    private readonly IFinanceService _financeService;
    private readonly ICurrentUserContext _currentUser;

    public ExpenseController(IFinanceService financeService, ICurrentUserContext currentUser)
    {
        _financeService = financeService;
        _currentUser = currentUser;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);
        var vm = new ExpenseFormViewModel { Existing = await _financeService.GetExpensesAsync(userId, ct) };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseFormViewModel vm, CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);

        if (!ModelState.IsValid)
        {
            vm.Existing = await _financeService.GetExpensesAsync(userId, ct);
            return View("Index", vm);
        }

        await _financeService.AddExpenseAsync(userId, new CreateExpenseDto(vm.Amount, vm.Category, vm.Date, vm.Note), ct);
        return RedirectToAction(nameof(Index));
    }
}
