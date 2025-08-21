using Fintrox.Domain.Accounting;

namespace Fintrox.Application.Accounting;

public interface IAccountingService
{
	Task<JournalEntry> PostSimpleAsync(
		DateTime date, string number, string description,
		Guid debitAccountId, Guid creditAccountId, decimal amount, Guid? counterpartyId = null);

	Task<TrialBalanceDto> GetTrialBalanceAsync(DateTime from, DateTime to);
}

public record TrialBalanceRow(string AccountCode, string AccountName, decimal Debit, decimal Credit, decimal Balance);
public record TrialBalanceDto(DateTime From, DateTime To, IReadOnlyList<TrialBalanceRow> Rows, decimal TotalDebit, decimal TotalCredit);
