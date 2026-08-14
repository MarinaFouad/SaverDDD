namespace WealthOS.Application.DTOs;

public record SavingDto(Guid Id, decimal Amount, DateOnly Date, string? Note);

public record CreateSavingDto(decimal Amount, DateOnly Date, string? Note);
