namespace WealthOS.Application.Services;

using WealthOS.Application.DTOs;
using WealthOS.Application.Interfaces;
using WealthOS.Domain.Entities;
using WealthOS.Domain.Enums;
using WealthOS.Domain.Interfaces;
using WealthOS.Domain.Services;
using WealthOS.Domain.ValueObjects;

public class FinanceService : IFinanceService
{
    private readonly IIncomeRepository _incomeRepo;
    private readonly IExpenseRepository _expenseRepo;
    private readonly ISavingRepository _savingRepo;
    private readonly IGoalRepository _goalRepo;
    private readonly IWeeklyTaskRepository _taskRepo;
    private readonly FinancialCalculator _calculator;
    private readonly WeeklyTaskSuggestionEngine _suggestionEngine;
    private readonly IAiCoachService _aiCoach;

    public FinanceService(
        IIncomeRepository incomeRepo,
        IExpenseRepository expenseRepo,
        ISavingRepository savingRepo,
        IGoalRepository goalRepo,
        IWeeklyTaskRepository taskRepo,
        FinancialCalculator calculator,
        WeeklyTaskSuggestionEngine suggestionEngine,
        IAiCoachService aiCoach)
    {
        _incomeRepo = incomeRepo;
        _expenseRepo = expenseRepo;
        _savingRepo = savingRepo;
        _goalRepo = goalRepo;
        _taskRepo = taskRepo;
        _calculator = calculator;
        _suggestionEngine = suggestionEngine;
        _aiCoach = aiCoach;
    }

    // ---------- Income ----------

    public async Task<IncomeDto> AddIncomeAsync(Guid userId, CreateIncomeDto dto, CancellationToken ct = default)
    {
        var entry = new IncomeEntry(userId, Money.Of(dto.Amount), dto.Source, dto.Date, dto.Note);
        await _incomeRepo.AddAsync(entry, ct);
        await _incomeRepo.SaveChangesAsync(ct);
        return ToDto(entry);
    }

    public async Task<List<IncomeDto>> GetIncomesAsync(Guid userId, CancellationToken ct = default)
    {
        var entries = await _incomeRepo.GetByUserAsync(userId, ct);
        return entries.OrderByDescending(e => e.Date).Select(ToDto).ToList();
    }

    // ---------- Expenses ----------

    public async Task<ExpenseDto> AddExpenseAsync(Guid userId, CreateExpenseDto dto, CancellationToken ct = default)
    {
        var entry = new ExpenseEntry(userId, Money.Of(dto.Amount), dto.Category, dto.Date, dto.Note);
        await _expenseRepo.AddAsync(entry, ct);
        await _expenseRepo.SaveChangesAsync(ct);
        return ToDto(entry);
    }

    public async Task<List<ExpenseDto>> GetExpensesAsync(Guid userId, CancellationToken ct = default)
    {
        var entries = await _expenseRepo.GetByUserAsync(userId, ct);
        return entries.OrderByDescending(e => e.Date).Select(ToDto).ToList();
    }

    // ---------- Savings ----------

    public async Task<SavingDto> AddSavingAsync(Guid userId, CreateSavingDto dto, CancellationToken ct = default)
    {
        var entry = new SavingEntry(userId, Money.Of(dto.Amount), dto.Date, dto.Note);
        await _savingRepo.AddAsync(entry, ct);
        await _savingRepo.SaveChangesAsync(ct);
        return ToDto(entry);
    }

    public async Task<List<SavingDto>> GetSavingsAsync(Guid userId, CancellationToken ct = default)
    {
        var entries = await _savingRepo.GetByUserAsync(userId, ct);
        return entries.OrderByDescending(e => e.Date).Select(ToDto).ToList();
    }

    // ---------- Goal ----------

    public async Task<GoalDto> SetGoalAsync(Guid userId, CreateGoalDto dto, CancellationToken ct = default)
    {
        // Deactivate any existing active goal — MVP supports one active goal at a time.
        var existing = await _goalRepo.GetActiveByUserAsync(userId, ct);
        existing?.Deactivate();

        var goal = new FinancialGoal(userId, dto.Name, Money.Of(dto.TargetAmount), dto.TargetDate, dto.TargetSavingsRatePercent);
        await _goalRepo.AddAsync(goal, ct);
        await _goalRepo.SaveChangesAsync(ct);
        return ToDto(goal);
    }

    public async Task<GoalDto?> GetActiveGoalAsync(Guid userId, CancellationToken ct = default)
    {
        var goal = await _goalRepo.GetActiveByUserAsync(userId, ct);
        return goal is null ? null : ToDto(goal);
    }

    // ---------- Dashboard ----------

    public async Task<DashboardDto> GetDashboardAsync(Guid userId, CancellationToken ct = default)
    {
        var (from, to) = CurrentMonthRange();

        var incomes = await _incomeRepo.GetByUserInRangeAsync(userId, from, to, ct);
        var expenses = await _expenseRepo.GetByUserInRangeAsync(userId, from, to, ct);
        var savings = await _savingRepo.GetByUserInRangeAsync(userId, from, to, ct);

        var allIncomes = await _incomeRepo.GetByUserAsync(userId, ct);
        var allExpenses = await _expenseRepo.GetByUserAsync(userId, ct);
        var allSavings = await _savingRepo.GetByUserAsync(userId, ct);

        var monthlyIncome = Sum(incomes.Select(i => i.Amount.Amount));
        var monthlyExpenses = Sum(expenses.Select(e => e.Amount.Amount));
        var monthlySavings = Sum(savings.Select(s => s.Amount.Amount));

        var surplus = _calculator.CalculateMonthlySurplus(Money.Of(monthlyIncome), Money.Of(monthlyExpenses));
        var savingsRate = _calculator.CalculateSavingsRatePercent(Money.Of(monthlyIncome), Money.Of(monthlySavings));

        // Net worth (MVP definition): total money ever saved - total money ever spent beyond income is
        // already reflected in surplus, so we approximate current net worth as cumulative savings.
        var currentNetWorth = Sum(allSavings.Select(s => s.Amount.Amount));

        var goal = await _goalRepo.GetActiveByUserAsync(userId, ct);
        int? monthsToGoal = null;
        decimal goalProgress = 0;
        if (goal is not null)
        {
            monthsToGoal = _calculator.MonthsToGoal(currentNetWorth, goal.TargetAmount.Amount, surplus);
            goalProgress = goal.TargetAmount.Amount == 0
                ? 0
                : Math.Round(Math.Min(currentNetWorth / goal.TargetAmount.Amount * 100m, 100m), 1);
        }

        var projection = _calculator.ProjectNetWorth(currentNetWorth, surplus, months: 12)
            .Select(p => new ProjectionPointDto(p.MonthOffset, Math.Round(p.ProjectedNetWorth, 2)))
            .ToList();

        var weekStart = CurrentWeekStart();
        var weekTasks = await _taskRepo.GetByUserForWeekAsync(userId, weekStart, ct);

        return new DashboardDto(
            MonthlyIncome: monthlyIncome,
            MonthlyExpenses: monthlyExpenses,
            MonthlySavings: monthlySavings,
            MonthlySurplus: surplus,
            SavingsRatePercent: savingsRate,
            CurrentNetWorth: currentNetWorth,
            GoalTargetAmount: goal?.TargetAmount.Amount,
            GoalName: goal?.Name,
            MonthsToGoal: monthsToGoal,
            GoalProgressPercent: goalProgress,
            Projection: projection,
            ThisWeekTasks: weekTasks.Select(ToDto).ToList()
        );
    }

    // ---------- Weekly tasks ----------

    public async Task<List<WeeklyTaskDto>> GenerateWeeklyTasksAsync(Guid userId, CancellationToken ct = default)
    {
        var weekStart = CurrentWeekStart();

        // Idempotent: don't duplicate tasks if already generated for this week.
        var existing = await _taskRepo.GetByUserForWeekAsync(userId, weekStart, ct);
        if (existing.Count > 0) return existing.Select(ToDto).ToList();

        var (from, to) = CurrentMonthRange();
        var incomes = await _incomeRepo.GetByUserInRangeAsync(userId, from, to, ct);
        var expenses = await _expenseRepo.GetByUserInRangeAsync(userId, from, to, ct);
        var savings = await _savingRepo.GetByUserInRangeAsync(userId, from, to, ct);

        var monthlyIncome = Sum(incomes.Select(i => i.Amount.Amount));
        var monthlyExpenses = Sum(expenses.Select(e => e.Amount.Amount));
        var monthlySavings = Sum(savings.Select(s => s.Amount.Amount));

        var surplus = _calculator.CalculateMonthlySurplus(Money.Of(monthlyIncome), Money.Of(monthlyExpenses));
        var savingsRate = _calculator.CalculateSavingsRatePercent(Money.Of(monthlyIncome), Money.Of(monthlySavings));

        var goal = await _goalRepo.GetActiveByUserAsync(userId, ct);
        var targetRate = goal?.TargetSavingsRatePercent ?? 20m; // sensible default if no goal set yet

        var topCategory = expenses
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount.Amount) })
            .OrderByDescending(x => x.Total)
            .FirstOrDefault();

        var topShare = topCategory is not null && monthlyExpenses > 0
            ? Math.Round(topCategory.Total / monthlyExpenses * 100m, 1)
            : 0m;

        var expenseBreakdown = expenses
            .GroupBy(e => e.Category)
            .Select(g => (Category: g.Key.ToString(), Amount: g.Sum(e => e.Amount.Amount)))
            .ToList();

        var aiContext = new AiCoachContextDto(
            MonthlyIncome: monthlyIncome,
            MonthlyExpenses: monthlyExpenses,
            MonthlySavings: monthlySavings,
            MonthlySurplus: surplus,
            SavingsRatePercent: savingsRate,
            TargetSavingsRatePercent: targetRate,
            GoalName: goal?.Name,
            GoalTargetAmount: goal?.TargetAmount.Amount,
            ExpenseBreakdown: expenseBreakdown);

        List<WeeklyTask> tasks;
        var aiSuggestions = await _aiCoach.TrySuggestTasksAsync(aiContext, ct);

        if (aiSuggestions is { Count: > 0 })
        {
            tasks = aiSuggestions
                .Select(s => new WeeklyTask(
                    userId,
                    s.Title,
                    s.Description,
                    Enum.TryParse<TaskType>(s.Type, ignoreCase: true, out var type) ? type : TaskType.ReduceExpenses,
                    weekStart,
                    TaskSource.Ai))
                .ToList();
        }
        else
        {
            // AI coach disabled, unreachable, or returned nothing usable — deterministic fallback.
            tasks = _suggestionEngine.Suggest(
                userId, weekStart, surplus, savingsRate, targetRate, topShare, topCategory?.Category.ToString());
        }

        await _taskRepo.AddRangeAsync(tasks, ct);
        await _taskRepo.SaveChangesAsync(ct);

        return tasks.Select(ToDto).ToList();
    }

    public async Task<List<WeeklyTaskDto>> GetCurrentWeekTasksAsync(Guid userId, CancellationToken ct = default)
    {
        var tasks = await _taskRepo.GetByUserForWeekAsync(userId, CurrentWeekStart(), ct);
        return tasks.Select(ToDto).ToList();
    }

    public async Task CompleteTaskAsync(Guid userId, Guid taskId, CancellationToken ct = default)
    {
        var task = await _taskRepo.GetByIdAsync(taskId, ct);
        if (task is null || task.UserId != userId) return;
        task.MarkCompleted();
        await _taskRepo.SaveChangesAsync(ct);
    }

    public async Task UncompleteTaskAsync(Guid userId, Guid taskId, CancellationToken ct = default)
    {
        var task = await _taskRepo.GetByIdAsync(taskId, ct);
        if (task is null || task.UserId != userId) return;
        task.MarkIncomplete();
        await _taskRepo.SaveChangesAsync(ct);
    }

    public async Task<WeeklyTaskDto> CreateManualTaskAsync(Guid userId, CreateWeeklyTaskDto dto, CancellationToken ct = default)
    {
        var task = new WeeklyTask(userId, dto.Title, dto.Description, dto.Type, CurrentWeekStart(), TaskSource.Manual);
        await _taskRepo.AddAsync(task, ct);
        await _taskRepo.SaveChangesAsync(ct);
        return ToDto(task);
    }

    public async Task<WeeklyTaskDto?> GetTaskAsync(Guid userId, Guid taskId, CancellationToken ct = default)
    {
        var task = await _taskRepo.GetByIdAsync(taskId, ct);
        if (task is null || task.UserId != userId) return null;
        return ToDto(task);
    }

    public async Task<WeeklyTaskDto?> UpdateTaskAsync(Guid userId, Guid taskId, UpdateWeeklyTaskDto dto, CancellationToken ct = default)
    {
        var task = await _taskRepo.GetByIdAsync(taskId, ct);
        if (task is null || task.UserId != userId) return null;

        task.Update(dto.Title, dto.Description, dto.Type);
        await _taskRepo.SaveChangesAsync(ct);
        return ToDto(task);
    }

    public async Task DeleteTaskAsync(Guid userId, Guid taskId, CancellationToken ct = default)
    {
        var task = await _taskRepo.GetByIdAsync(taskId, ct);
        if (task is null || task.UserId != userId) return;
        _taskRepo.Remove(task);
        await _taskRepo.SaveChangesAsync(ct);
    }

    // ---------- helpers ----------

    private static decimal Sum(IEnumerable<decimal> values) => values.Aggregate(0m, (acc, v) => acc + v);

    private static (DateOnly from, DateOnly to) CurrentMonthRange()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = new DateOnly(today.Year, today.Month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        return (from, to);
    }

    private static DateOnly CurrentWeekStart()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var diff = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
        if (diff < 0) diff += 7;
        return today.AddDays(-diff);
    }

    private static IncomeDto ToDto(IncomeEntry e) => new(e.Id, e.Amount.Amount, e.Source, e.Date, e.Note);
    private static ExpenseDto ToDto(ExpenseEntry e) => new(e.Id, e.Amount.Amount, e.Category, e.Date, e.Note);
    private static SavingDto ToDto(SavingEntry e) => new(e.Id, e.Amount.Amount, e.Date, e.Note);
    private static GoalDto ToDto(FinancialGoal g) => new(g.Id, g.Name, g.TargetAmount.Amount, g.TargetDate, g.TargetSavingsRatePercent, g.IsActive);
    private static WeeklyTaskDto ToDto(WeeklyTask t) => new(t.Id, t.Title, t.Description, t.Type, t.Source, t.WeekStartDate, t.IsCompleted);
}
