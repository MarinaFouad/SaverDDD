namespace WealthOS.Application.DTOs;

using WealthOS.Domain.Enums;

public record ExpenseDto(Guid Id, decimal Amount, ExpenseCategory Category, DateOnly Date, string? Note);

public record CreateExpenseDto(decimal Amount, ExpenseCategory Category, DateOnly Date, string? Note);
