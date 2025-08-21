using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fintrox.Domain.Constants;
using Fintrox.Domain.Partners;

namespace Fintrox.Domain.Accounting;

public class JournalEntry
{
	[Key] public Guid Id { get; set; }

	public DateTime Date { get; set; }

	[Required, MaxLength(EntityConstraints.JournalNumberMaxLength)]
	public string Number { get; set; } = default!;

	[MaxLength(EntityConstraints.JournalDescriptionMaxLength)]
	public string? Description { get; set; }

	public Guid? CounterpartyId { get; set; }
	public Counterparty? Counterparty { get; set; }

	public ICollection<JournalLine> Lines { get; set; } = new List<JournalLine>();
}

public class JournalLine
{
	[Key] public Guid Id { get; set; }

	[Required] public Guid JournalEntryId { get; set; }
	public JournalEntry JournalEntry { get; set; } = default!;

	[Required] public Guid DebitAccountId { get; set; }
	public Account DebitAccount { get; set; } = default!;

	[Required] public Guid CreditAccountId { get; set; }
	public Account CreditAccount { get; set; } = default!;

	[Column(TypeName = EntityConstraints.DecimalColumnType)]
	public decimal Amount { get; set; }

	[Column(TypeName = EntityConstraints.DecimalColumnType)]
	public decimal? VATAmount { get; set; }
}
