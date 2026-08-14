namespace WealthOS.Web.Models;

using System.ComponentModel.DataAnnotations;
using WealthOS.Application.DTOs;
using WealthOS.Domain.Enums;

public class IncomeFormViewModel
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    public IncomeSource Source { get; set; } = IncomeSource.Salary;

    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [StringLength(500)]
    public string? Note { get; set; }

    public List<IncomeDto> Existing { get; set; } = new();
}
