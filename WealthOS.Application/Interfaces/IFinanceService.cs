namespace WealthOS.Application.Interfaces;

using WealthOS.Application.DTOs;

/// <summary>
/// Application-layer facade for everything the MVC controllers need: recording
/// income/expenses/savings, managing the financial goal, and reading the dashboard.
/// Orchestrates domain entities + repositories; contains no business rules itself
/// (those live in WealthOS.Domain.Services.FinancialCalculator and the entities).
/// </summary>
public interface IFinanceService
{
    Task<IncomeDto> AddIncomeAsync(Guid userId, CreateIncomeDto dto, CancellationToken ct = default);
    Task<List<IncomeDto>> GetIncomesAsync(Guid userId, CancellationToken ct = default);

    Task<ExpenseDto> AddExpenseAsync(Guid userId, CreateExpenseDto dto, CancellationToken ct = default);
    Task<List<ExpenseDto>> GetExpensesAsync(Guid userId, CancellationToken ct = default);

    Task<SavingDto> AddSavingAsync(Guid userId, CreateSavingDto dto, CancellationToken ct = default);
    Task<List<SavingDto>> GetSavingsAsync(Guid userId, CancellationToken ct = default);

    Task<GoalDto> SetGoalAsync(Guid userId, CreateGoalDto dto, CancellationToken ct = default);
    Task<GoalDto?> GetActiveGoalAsync(Guid userId, CancellationToken ct = default);

    Task<DashboardDto> GetDashboardAsync(Guid userId, CancellationToken ct = default);

    Task<List<WeeklyTaskDto>> GenerateWeeklyTasksAsync(Guid userId, CancellationToken ct = default);
    Task<List<WeeklyTaskDto>> GetCurrentWeekTasksAsync(Guid userId, CancellationToken ct = default);
    Task CompleteTaskAsync(Guid userId, Guid taskId, CancellationToken ct = default);
    Task UncompleteTaskAsync(Guid userId, Guid taskId, CancellationToken ct = default);
    Task<WeeklyTaskDto> CreateManualTaskAsync(Guid userId, CreateWeeklyTaskDto dto, CancellationToken ct = default);
    Task<WeeklyTaskDto?> GetTaskAsync(Guid userId, Guid taskId, CancellationToken ct = default);
    Task<WeeklyTaskDto?> UpdateTaskAsync(Guid userId, Guid taskId, UpdateWeeklyTaskDto dto, CancellationToken ct = default);
    Task DeleteTaskAsync(Guid userId, Guid taskId, CancellationToken ct = default);
}
