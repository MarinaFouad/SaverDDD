namespace WealthOS.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Interfaces;
using WealthOS.Web.Models;
using WealthOS.Web.Services;

public class DashboardController : Controller
{
    private readonly IFinanceService _financeService;
    private readonly ICurrentUserContext _currentUser;

    public DashboardController(IFinanceService financeService, ICurrentUserContext currentUser)
    {
        _financeService = financeService;
        _currentUser = currentUser;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = await _currentUser.GetCurrentUserIdAsync(ct);

        // Make sure this week's suggested tasks exist so the dashboard always has something to show.
        await _financeService.GenerateWeeklyTasksAsync(userId, ct);

        var dashboard = await _financeService.GetDashboardAsync(userId, ct);
        return View(new DashboardViewModel { Dashboard = dashboard });
    }
}
