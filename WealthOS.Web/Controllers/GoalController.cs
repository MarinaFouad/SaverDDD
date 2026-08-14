namespace WealthOS.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.DTOs;
using WealthOS.Application.Interfaces;
using WealthOS.Web.Models;
using WealthOS.Web.Services;

public class GoalController : Controller
{
    private readonly IFinanceService _financeService;
    private readonly ICurrentUserContext _currentUser;

    public GoalController(IFinanceService financeService, ICurrentUserContext currentUser)
    {
        _financeService = financeService;
        _currentUser = currentUser;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);
        var active = await _financeService.GetActiveGoalAsync(userId, ct);
        var vm = new GoalFormViewModel { Active = active };
        if (active is not null)
        {
            vm.Name = active.Name;
            vm.TargetAmount = active.TargetAmount;
            vm.TargetDate = active.TargetDate;
            vm.TargetSavingsRatePercent = active.TargetSavingsRatePercent;
        }
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(GoalFormViewModel vm, CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);

        if (!ModelState.IsValid)
        {
            vm.Active = await _financeService.GetActiveGoalAsync(userId, ct);
            return View("Index", vm);
        }

        await _financeService.SetGoalAsync(
            userId,
            new CreateGoalDto(vm.Name, vm.TargetAmount, vm.TargetDate, vm.TargetSavingsRatePercent),
            ct);

        return RedirectToAction("Index", "Dashboard");
    }
}
