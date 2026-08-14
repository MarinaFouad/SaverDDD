namespace WealthOS.Infrastructure.AiCoach;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using WealthOS.Application.DTOs;

/// <summary>Parses the {"tasks":[...]} JSON shape every provider is prompted to return, tolerating minor formatting slips.</summary>
internal static class AiCoachResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static List<AiSuggestedTaskDto>? Parse(string raw, ILogger logger)
    {
        var cleaned = StripCodeFences(raw).Trim();

        try
        {
            var parsed = JsonSerializer.Deserialize<AiTasksEnvelope>(cleaned, JsonOptions);
            var tasks = parsed?.Tasks;
            if (tasks is null || tasks.Count == 0) return null;

            return tasks
                .Where(t => !string.IsNullOrWhiteSpace(t.Title) && !string.IsNullOrWhiteSpace(t.Description))
                .Take(4)
                .ToList();
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Could not parse AI coach response as JSON; falling back to rule-based tasks. Raw: {Raw}", cleaned);
            return null;
        }
    }

    private static string StripCodeFences(string text)
    {
        var trimmed = text.Trim();
        if (trimmed.StartsWith("```"))
        {
            var firstNewline = trimmed.IndexOf('\n');
            if (firstNewline >= 0) trimmed = trimmed[(firstNewline + 1)..];
            var closingFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
            if (closingFence >= 0) trimmed = trimmed[..closingFence];
        }
        return trimmed;
    }

    private record AiTasksEnvelope([property: JsonPropertyName("tasks")] List<AiSuggestedTaskDto>? Tasks);
}
