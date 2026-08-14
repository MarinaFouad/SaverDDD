namespace WealthOS.Web.Models;

using System.ComponentModel.DataAnnotations;
using WealthOS.Application.DTOs;

public class GoalFormViewModel
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than 0.")]
    public decimal TargetAmount { get; set; }

    [Required]
    public DateOnly TargetDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

    [Range(0, 100, ErrorMessage = "Must be between 0 and 100.")]
    public decimal TargetSavingsRatePercent { get; set; } = 20;

    public GoalDto? Active { get; set; }
}
