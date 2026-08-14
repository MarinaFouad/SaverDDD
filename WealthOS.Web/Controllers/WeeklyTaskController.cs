namespace WealthOS.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.DTOs;
using WealthOS.Application.Interfaces;
using WealthOS.Web.Models;
using WealthOS.Web.Services;

public class WeeklyTaskController : Controller
{
    private readonly IFinanceService _financeService;
    private readonly ICurrentUserContext _currentUser;

    public WeeklyTaskController(IFinanceService financeService, ICurrentUserContext currentUser)
    {
        _financeService = financeService;
        _currentUser = currentUser;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);
        var tasks = await _financeService.GetCurrentWeekTasksAsync(userId, ct);
        return View(tasks);
    }

    // ---------- Create ----------

    public IActionResult Create() => View(new WeeklyTaskFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WeeklyTaskFormViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        var userId = await _currentUser.GetCurrentUserIdAsync(ct);
        await _financeService.CreateManualTaskAsync(userId, new CreateWeeklyTaskDto(vm.Title, vm.Description ?? string.Empty, vm.Type), ct);
        return RedirectToAction(nameof(Index));
    }

    // ---------- Edit ----------

    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);
        var task = await _financeService.GetTaskAsync(userId, id, ct);
        if (task is null) return NotFound();

        return View(new WeeklyTaskFormViewModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Type = task.Type
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(WeeklyTaskFormViewModel vm, CancellationToken ct)
    {
        if (vm.Id is null) return NotFound();
        if (!ModelState.IsValid) return View(vm);

        var userId = await _currentUser.GetCurrentUserIdAsync(ct);
        var updated = await _financeService.UpdateTaskAsync(userId, vm.Id.Value, new UpdateWeeklyTaskDto(vm.Title, vm.Description ?? string.Empty, vm.Type), ct);
        if (updated is null) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    // ---------- Delete ----------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);
        await _financeService.DeleteTaskAsync(userId, id, ct);
        return RedirectToAction(nameof(Index));
    }

    // ---------- Complete / Uncomplete ----------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);
        await _financeService.CompleteTaskAsync(userId, id, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Uncomplete(Guid id, CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);
        await _financeService.UncompleteTaskAsync(userId, id, ct);
        return RedirectToAction(nameof(Index));
    }
}
