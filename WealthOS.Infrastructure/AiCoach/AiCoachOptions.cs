namespace WealthOS.Infrastructure.AiCoach;

/// <summary>Bound from the "AiCoach" section of appsettings.json.</summary>
public class AiCoachOptions
{
    public const string SectionName = "AiCoach";

    /// <summary>"None" (rule engine only), "Ollama" (free, local, no key), or "Groq" (free tier, needs an API key).</summary>
    public string Provider { get; set; } = "None";

    public OllamaOptions Ollama { get; set; } = new();
    public GroqOptions Groq { get; set; } = new();

    public class OllamaOptions
    {
        public string BaseUrl { get; set; } = "http://localhost:11434";
        public string Model { get; set; } = "llama3.1";
    }

    public class GroqOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "llama-3.1-8b-instant";
    }
}
