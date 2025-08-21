namespace Fintrox.Domain.Constants;

public static class EntityConstraints
{
	// Account
	public const int AccountCodeMaxLength = 10;
	public const int AccountNameMaxLength = 100;

	// Counterparty
	public const int CounterpartyNameMaxLength = 200;
	public const int CounterpartyBulstatMaxLength = 15;
	public const int CounterpartyVatMaxLength = 20;
	public const int CounterpartyEmailMaxLength = 150;

	// Invoice
	public const int InvoiceNumberMaxLength = 20;
	public const int InvoiceItemNameMaxLength = 150;

	// JournalEntry
	public const int JournalNumberMaxLength = 20;
	public const int JournalDescriptionMaxLength = 300;

	// General money precision
	public const string DecimalColumnType = "decimal(18,2)";
}
