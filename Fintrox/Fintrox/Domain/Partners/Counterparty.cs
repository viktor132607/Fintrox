using System.ComponentModel.DataAnnotations;
using Fintrox.Domain.Constants;

namespace Fintrox.Domain.Partners;

public class Counterparty
{
	[Key]
	public Guid Id { get; set; }

	[Required]
	[MaxLength(EntityConstraints.CounterpartyNameMaxLength)]
	public string Name { get; set; } = default!;

	[MaxLength(EntityConstraints.CounterpartyBulstatMaxLength)]
	public string Bulstat { get; set; } = "";

	[MaxLength(EntityConstraints.CounterpartyVatMaxLength)]
	public string VATNumber { get; set; } = "";

	[MaxLength(EntityConstraints.CounterpartyEmailMaxLength)]
	public string Email { get; set; } = "";

	public string Address { get; set; } = "";

	public bool IsCustomer { get; set; }
	public bool IsSupplier { get; set; }
}
