namespace WealthOS.Web.Models;

using System.ComponentModel.DataAnnotations;
using WealthOS.Domain.Enums;

public class WeeklyTaskFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public TaskType Type { get; set; } = TaskType.ReduceExpenses;
}
