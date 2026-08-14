namespace WealthOS.Domain.Enums;

/// <summary>Where a weekly task came from — lets the UI show "AI coach" / "Rules" / "Manual".</summary>
public enum TaskSource
{
    RuleBased = 1,
    Ai = 2,
    Manual = 3
}
