using System.ComponentModel.DataAnnotations;
using Fintrox.Domain.Constants;

namespace Fintrox.Domain.Accounting;

public enum AccountType { Asset, Liability, Equity, Income, Expense, OffBalance }

public class Account
{
	[Key]
	public Guid Id { get; set; }

	[Required]
	[MaxLength(EntityConstraints.AccountCodeMaxLength)]
	public string Code { get; set; } = default!;

	[Required]
	[MaxLength(EntityConstraints.AccountNameMaxLength)]
	public string Name { get; set; } = default!;

	[Required]
	public AccountType Type { get; set; }

	public Guid? ParentId { get; set; }
	public Account? Parent { get; set; }
	public ICollection<Account> Children { get; set; } = new List<Account>();

	public bool IsActive { get; set; } = true;
}
