namespace WealthOS.Web.Models;

using System.ComponentModel.DataAnnotations;
using WealthOS.Application.DTOs;
using WealthOS.Domain.Enums;

public class ExpenseFormViewModel
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    public ExpenseCategory Category { get; set; } = ExpenseCategory.Other;

    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [StringLength(500)]
    public string? Note { get; set; }

    public List<ExpenseDto> Existing { get; set; } = new();
}
