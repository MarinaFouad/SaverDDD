namespace WealthOS.Infrastructure.AiCoach;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WealthOS.Application.DTOs;
using WealthOS.Application.Interfaces;

/// <summary>
/// Calls Groq's OpenAI-compatible chat completions API (https://console.groq.com) — free
/// tier, no credit card required, just a free API key. Good option when you don't have a
/// machine that can run local models for Ollama.
/// </summary>
public class GroqAiCoachService : IAiCoachService
{
    private readonly HttpClient _http;
    private readonly AiCoachOptions _options;
    private readonly ILogger<GroqAiCoachService> _logger;

    public GroqAiCoachService(HttpClient http, IOptions<AiCoachOptions> options, ILogger<GroqAiCoachService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<List<AiSuggestedTaskDto>?> TrySuggestTasksAsync(AiCoachContextDto context, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Groq.ApiKey))
        {
            _logger.LogWarning("Groq API key not configured; falling back to rule-based tasks.");
            return null;
        }

        try
        {
            var request = new GroqChatRequest(
                Model: _options.Groq.Model,
                Messages: new[]
                {
                    new GroqMessage("system", AiCoachPromptBuilder.SystemPrompt),
                    new GroqMessage("user", AiCoachPromptBuilder.BuildUserPrompt(context))
                },
                ResponseFormat: new GroqResponseFormat("json_object"));

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.Groq.ApiKey);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(20));

            using var response = await _http.SendAsync(httpRequest, cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Groq returned {Status}; falling back to rule-based tasks.", response.StatusCode);
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<GroqChatResponse>(cancellationToken: cts.Token);
            var content = payload?.Choices?.FirstOrDefault()?.Message?.Content;
            if (content is null) return null;

            return AiCoachResponseParser.Parse(content, _logger);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Groq AI coach call failed; falling back to rule-based tasks.");
            return null;
        }
    }

    private record GroqMessage([property: JsonPropertyName("role")] string Role, [property: JsonPropertyName("content")] string Content);
    private record GroqResponseFormat([property: JsonPropertyName("type")] string Type);
    private record GroqChatRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] GroqMessage[] Messages,
        [property: JsonPropertyName("response_format")] GroqResponseFormat ResponseFormat);

    private record GroqChatResponse([property: JsonPropertyName("choices")] List<GroqChoice>? Choices);
    private record GroqChoice([property: JsonPropertyName("message")] GroqMessage? Message);
}
