# WealthOS — MVP

A personal finance app: log income/expenses/savings, set a financial goal, see your
monthly surplus and savings rate, get weekly tasks to hit your goal faster, and view a
dashboard with Net Worth + progress + a 12‑month projection.

Built with **.NET 8**, **ASP.NET Core MVC**, and **Domain-Driven Design** layering.

> ⚠️ This code was written without a .NET SDK or NuGet access in the sandbox that generated
> it, so it has **not** been compiled or run yet. It's structurally complete and should build
> cleanly, but run `dotnet build` first and fix anything your local SDK/NuGet feed flags.

## Architecture (DDD layers)

```
WealthOS.Domain          → Entities, Value Objects, Enums, Repository interfaces,
                            FinancialCalculator (pure business rules). No dependencies.
WealthOS.Application      → DTOs, IFinanceService (use-case orchestration),
                            WeeklyTaskSuggestionEngine. Depends only on Domain.
WealthOS.Infrastructure    → EF Core DbContext, entity configurations (Fluent API),
                            repository implementations (SQLite). Implements Domain interfaces.
WealthOS.Web               → ASP.NET Core MVC controllers, Razor views, DI composition root
                            (Program.cs). Depends on Application's abstractions.
```

Dependency direction: `Web → Application → Domain`, with `Infrastructure` plugged in only at
the composition root (`Program.cs`) via `AddInfrastructure(...)`. Controllers never touch EF
Core or the DbContext directly — they only see `IFinanceService`.

## The 5 MVP features (from the original brief)

1. **Income / expenses / savings input** — `IncomeController`, `ExpenseController`,
   `SavingController` + their forms and ledger-style history tables.
2. **Financial goal** — `GoalController`; one active goal at a time (name, target amount,
   target date, target savings rate %).
3. **Monthly surplus & savings-rate calculation** — `FinancialCalculator` in the Domain layer
   (`CalculateMonthlySurplus`, `CalculateSavingsRatePercent`) — pure, unit-testable logic.
4. **Weekly task suggestions** — two interchangeable strategies behind `IAiCoachService`:
   - **AI coach** (optional, free): calls a free LLM to generate specific, numbers-aware
     tasks. See "Enabling the free AI coach" below.
   - **Rule engine** (always available, zero setup): `WeeklyTaskSuggestionEngine`
     (Application layer) turns this month's surplus/savings-rate/top-expense-category into
     2–4 concrete weekly tasks using fixed rules.
   The AI coach is tried first when enabled; if it's disabled, unreachable, or returns
   something unparsable, `FinanceService` transparently falls back to the rule engine — the
   dashboard never breaks because of the AI. Each generated task is tagged with its
   `TaskSource` (`Ai` or `RuleBased`) and the UI shows an "AI coach" badge accordingly.
5. **Dashboard** — Net Worth, goal progress bar, a simple straight-line 12-month projection
   (inline SVG, no chart library needed), and this week's tasks.

**Deferred to phase 2** (as in the original note): paid subscription. The AI coach itself is
now implemented (see below) using free providers only.

## Enabling the free AI coach

By default `AiCoach:Provider` is `"None"` in `appsettings.json`, so the app runs entirely on
the deterministic rule engine — no setup required. To turn on the AI coach, pick one:

### Option A — Ollama (fully free, runs on your machine, no API key)

1. Install Ollama: https://ollama.com
2. Pull a model: `ollama pull llama3.1` (or a smaller one like `llama3.2` if you're on modest hardware)
3. Make sure Ollama is running (it starts a local server on `http://localhost:11434` by default)
4. In `WealthOS.Web/appsettings.json`, set:
   ```json
   "AiCoach": { "Provider": "Ollama", "Ollama": { "BaseUrl": "http://localhost:11434", "Model": "llama3.1" } }
   ```

### Option B — Groq (free cloud API, no credit card, needs a free API key)

1. Get a free API key at https://console.groq.com
2. In `WealthOS.Web/appsettings.json` (or better, via `dotnet user-secrets` so the key isn't
   committed), set:
   ```json
   "AiCoach": { "Provider": "Groq", "Groq": { "ApiKey": "YOUR_KEY", "Model": "llama-3.1-8b-instant" } }
   ```
   To use user-secrets instead of putting the key in appsettings.json:
   ```bash
   cd WealthOS.Web
   dotnet user-secrets set "AiCoach:Groq:ApiKey" "YOUR_KEY"
   ```

Both providers share the same prompt (`WealthOS.Infrastructure/AiCoach/AiCoachPromptBuilder.cs`)
and the same JSON response contract, so switching providers is just a config change — no code
changes needed. Every call has a 20-second timeout and never throws; on any failure it logs a
warning and the rule engine takes over silently.

## Running it

```bash
cd WealthOS
dotnet restore
dotnet build
dotnet run --project WealthOS.Web
```

The app uses **SQLite** (`wealthos.db`, created automatically on first run via
`Database.EnsureCreated()` — no migrations needed for the MVP). Open the printed
`https://localhost:xxxx` URL; it redirects to the Dashboard.

There's no authentication yet — a single "Demo User" is created automatically on first run
(`WealthOS.Web/Services/CurrentUserContext.cs`). Swap that class for real ASP.NET Core
Identity / OAuth when you're ready for multi-user support; nothing in Domain or Application
assumes single-tenancy, they just take a `Guid userId`.

## Suggested next steps

- Replace `Database.EnsureCreated()` with real EF Core migrations
  (`dotnet ef migrations add Init -p WealthOS.Infrastructure -s WealthOS.Web`).
- Add ASP.NET Core Identity (or an external IdP) and wire `ICurrentUserContext` to it.
- Add unit tests for `FinancialCalculator` and `WeeklyTaskSuggestionEngine` — both are pure
  and dependency-free by design, so they're cheap to test.
- Consider caching/rate-limiting AI coach calls if you add more users (Groq's free tier has
  request limits; Ollama is limited only by your own hardware).
- Phase 2: paid subscription (Stripe/Paymob integration + a `Subscription` entity gating
  premium features, e.g. making the AI coach itself a paid perk vs. the free rule engine).
