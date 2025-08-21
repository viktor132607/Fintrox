using Fintrox.Domain.Accounting;
using Fintrox.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Fintrox.Application.Accounting;

public class AccountingService : IAccountingService
{
	private readonly AppDbContext _db;
	public AccountingService(AppDbContext db) => _db = db;

	public async Task<JournalEntry> PostSimpleAsync(
		DateTime date, string number, string description,
		Guid debitAccountId, Guid creditAccountId, decimal amount, Guid? counterpartyId = null)
	{
		var entry = new JournalEntry
		{
			Id = Guid.NewGuid(),
			Date = date,
			Number = number,
			Description = description,
			CounterpartyId = counterpartyId
		};

		entry.Lines.Add(new JournalLine
		{
			Id = Guid.NewGuid(),
			DebitAccountId = debitAccountId,
			CreditAccountId = creditAccountId,
			Amount = amount
		});

		_db.JournalEntries.Add(entry);
		await _db.SaveChangesAsync();
		return entry;
	}

	public async Task<TrialBalanceDto> GetTrialBalanceAsync(DateTime from, DateTime to)
	{
		var lines = await _db.JournalLines
			.Include(l => l.DebitAccount)
			.Include(l => l.CreditAccount)
			.Include(l => l.JournalEntry)
			.Where(l => l.JournalEntry.Date >= from && l.JournalEntry.Date <= to)
			.ToListAsync();

		var map = new Dictionary<string, (string name, decimal d, decimal c)>(StringComparer.OrdinalIgnoreCase);

		foreach (var l in lines)
		{
			var da = l.DebitAccount;
			var ca = l.CreditAccount;

			if (!map.TryGetValue(da.Code, out var drec))
				map[da.Code] = (da.Name, l.Amount, 0);
			else
				map[da.Code] = (drec.name, drec.d + l.Amount, drec.c);

			if (!map.TryGetValue(ca.Code, out var crec))
				map[ca.Code] = (ca.Name, 0, l.Amount);
			else
				map[ca.Code] = (crec.name, crec.d, crec.c + l.Amount);
		}

		var rows = map.OrderBy(k => k.Key)
			.Select(k => new TrialBalanceRow(
				k.Key, k.Value.name, k.Value.d, k.Value.c, k.Value.d - k.Value.c))
			.ToList();

		var totalDebit = rows.Sum(r => r.Debit);
		var totalCredit = rows.Sum(r => r.Credit);

		return new TrialBalanceDto(from, to, rows, totalDebit, totalCredit);
	}
}
