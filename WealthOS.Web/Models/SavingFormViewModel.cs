namespace WealthOS.Web.Models;

using System.ComponentModel.DataAnnotations;
using WealthOS.Application.DTOs;

public class SavingFormViewModel
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [StringLength(500)]
    public string? Note { get; set; }

    public List<SavingDto> Existing { get; set; } = new();
}
