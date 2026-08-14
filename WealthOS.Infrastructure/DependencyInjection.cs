namespace WealthOS.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Interfaces;
using WealthOS.Domain.Interfaces;
using WealthOS.Infrastructure.AiCoach;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") ?? "Data Source=wealthos.db";

        services.AddDbContext<WealthOsDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IIncomeRepository, IncomeRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<ISavingRepository, SavingRepository>();
        services.AddScoped<IGoalRepository, GoalRepository>();
        services.AddScoped<IWeeklyTaskRepository, WeeklyTaskRepository>();
        services.AddScoped<INetWorthSnapshotRepository, NetWorthSnapshotRepository>();

        services.AddAiCoach(configuration);

        return services;
    }

    /// <summary>
    /// Wires up the AI coach based on AiCoach:Provider in config:
    /// "Ollama" = free local model (no API key, needs Ollama installed & running),
    /// "Groq"   = free-tier cloud API (needs a free API key from console.groq.com),
    /// anything else (or missing) = disabled, dashboard uses the rule-based engine only.
    /// </summary>
    private static IServiceCollection AddAiCoach(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiCoachOptions>(configuration.GetSection(AiCoachOptions.SectionName));
        var provider = configuration.GetSection(AiCoachOptions.SectionName)["Provider"] ?? "None";

        switch (provider.Trim().ToLowerInvariant())
        {
            case "ollama":
                services.AddHttpClient<IAiCoachService, OllamaAiCoachService>();
                break;
            case "groq":
                services.AddHttpClient<IAiCoachService, GroqAiCoachService>();
                break;
            default:
                services.AddScoped<IAiCoachService, NullAiCoachService>();
                break;
        }

        return services;
    }
}
