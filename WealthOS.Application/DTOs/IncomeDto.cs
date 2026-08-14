namespace WealthOS.Application.DTOs;

using WealthOS.Domain.Enums;

public record IncomeDto(Guid Id, decimal Amount, IncomeSource Source, DateOnly Date, string? Note);

public record CreateIncomeDto(decimal Amount, IncomeSource Source, DateOnly Date, string? Note);
