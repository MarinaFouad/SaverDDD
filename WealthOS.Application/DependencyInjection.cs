namespace WealthOS.Application;

using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Interfaces;
using WealthOS.Application.Services;
using WealthOS.Domain.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<FinancialCalculator>();
        services.AddScoped<WeeklyTaskSuggestionEngine>();
        services.AddScoped<IFinanceService, FinanceService>();
        return services;
    }
}
