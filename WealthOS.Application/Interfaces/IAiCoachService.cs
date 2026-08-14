namespace WealthOS.Application.Interfaces;

using WealthOS.Application.DTOs;

/// <summary>
/// Optional AI-powered replacement for the rule-based WeeklyTaskSuggestionEngine.
/// Implementations call an LLM (Ollama locally, Groq's free tier, etc.) and must
/// return null — never throw — on any failure (timeout, bad response, model down)
/// so FinanceService can fall back to the deterministic rule engine transparently.
/// </summary>
public interface IAiCoachService
{
    Task<List<AiSuggestedTaskDto>?> TrySuggestTasksAsync(AiCoachContextDto context, CancellationToken ct = default);
}
