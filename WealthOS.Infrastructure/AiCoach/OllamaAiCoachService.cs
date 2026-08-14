namespace WealthOS.Infrastructure.AiCoach;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WealthOS.Application.DTOs;
using WealthOS.Application.Interfaces;

/// <summary>
/// Calls a local Ollama instance (https://ollama.com) — completely free, no API key, no
/// signup, runs on your own machine. Requires Ollama to be installed and running with a
/// model pulled, e.g.: `ollama pull llama3.1`.
/// </summary>
public class OllamaAiCoachService : IAiCoachService
{
    private readonly HttpClient _http;
    private readonly AiCoachOptions _options;
    private readonly ILogger<OllamaAiCoachService> _logger;

    public OllamaAiCoachService(HttpClient http, IOptions<AiCoachOptions> options, ILogger<OllamaAiCoachService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<List<AiSuggestedTaskDto>?> TrySuggestTasksAsync(AiCoachContextDto context, CancellationToken ct = default)
    {
        try
        {
            var request = new OllamaGenerateRequest(
                Model: _options.Ollama.Model,
                Prompt: AiCoachPromptBuilder.SystemPrompt + "\n\n" + AiCoachPromptBuilder.BuildUserPrompt(context),
                Stream: false,
                Format: "json");

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(20));

            using var response = await _http.PostAsJsonAsync($"{_options.Ollama.BaseUrl}/api/generate", request, cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Ollama returned {Status}; falling back to rule-based tasks.", response.StatusCode);
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(cancellationToken: cts.Token);
            if (payload?.Response is null) return null;

            return AiCoachResponseParser.Parse(payload.Response, _logger);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ollama AI coach call failed; falling back to rule-based tasks.");
            return null;
        }
    }

    private record OllamaGenerateRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("prompt")] string Prompt,
        [property: JsonPropertyName("stream")] bool Stream,
        [property: JsonPropertyName("format")] string Format);

    private record OllamaGenerateResponse([property: JsonPropertyName("response")] string? Response);
}
