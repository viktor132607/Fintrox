using Fintrox.Application.Accounting;
using Fintrox.Infrastructure;
using Microsoft.EntityFrameworkCore;

public static class AccountingEndpoints
{
	public static IEndpointRouteBuilder MapAccounting(this IEndpointRouteBuilder app)
	{
		app.MapGet("/api/accounts", async (AppDbContext db) =>
			await db.Accounts.OrderBy(a => a.Code).ToListAsync());

		app.MapPost("/api/journal/simple", async (IAccountingService acc, PostDto dto) =>
		{
			var e = await acc.PostSimpleAsync(
				dto.Date, dto.Number, dto.Description,
				dto.DebitAccountId, dto.CreditAccountId, dto.Amount, dto.CounterpartyId);

			return Results.Ok(new { e.Id });
		});

		app.MapGet("/api/reports/trialbalance", async (IAccountingService acc, DateTime from, DateTime to) =>
			Results.Ok(await acc.GetTrialBalanceAsync(from, to)));

		return app;
	}

	public record PostDto(
		DateTime Date, string Number, string Description,
		Guid DebitAccountId, Guid CreditAccountId, decimal Amount, Guid? CounterpartyId);
}
