namespace WealthOS.Infrastructure.AiCoach;

using WealthOS.Application.DTOs;
using WealthOS.Application.Interfaces;

/// <summary>Used when AiCoach:Provider is "None" (or unset). Always defers to the rule-based engine.</summary>
public class NullAiCoachService : IAiCoachService
{
    public Task<List<AiSuggestedTaskDto>?> TrySuggestTasksAsync(AiCoachContextDto context, CancellationToken ct = default)
        => Task.FromResult<List<AiSuggestedTaskDto>?>(null);
}
