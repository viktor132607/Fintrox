using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fintrox.Domain.Constants;
using Fintrox.Domain.Partners;

namespace Fintrox.Domain.Sales;

public class Invoice
{
	[Key]
	public Guid Id { get; set; }

	[Required]
	[MaxLength(EntityConstraints.InvoiceNumberMaxLength)]
	public string Number { get; set; } = default!;

	public DateTime Date { get; set; }

	[Required]
	public Guid CounterpartyId { get; set; }
	public Counterparty Counterparty { get; set; } = default!;

	[Column(TypeName = EntityConstraints.DecimalColumnType)]
	public decimal TotalNet { get; set; }

	[Column(TypeName = EntityConstraints.DecimalColumnType)]
	public decimal TotalVAT { get; set; }

	[Column(TypeName = EntityConstraints.DecimalColumnType)]
	public decimal TotalGross { get; set; }

	public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
	public bool PostedToLedger { get; set; } = false;
}

public class InvoiceLine
{
	[Key]
	public Guid Id { get; set; }

	[Required]
	public Guid InvoiceId { get; set; }
	public Invoice Invoice { get; set; } = default!;

	[Required]
	[MaxLength(EntityConstraints.InvoiceItemNameMaxLength)]
	public string ItemName { get; set; } = default!;

	[Column(TypeName = EntityConstraints.DecimalColumnType)]
	public decimal Qty { get; set; }

	[Column(TypeName = EntityConstraints.DecimalColumnType)]
	public decimal UnitPrice { get; set; }

	[Column(TypeName = EntityConstraints.DecimalColumnType)]
	public decimal VATRate { get; set; }   // напр. 0.2m = 20%

	[NotMapped]
	public decimal Net => Math.Round(Qty * UnitPrice, 2);

	[NotMapped]
	public decimal VAT => Math.Round(Net * VATRate, 2);

	[NotMapped]
	public decimal Gross => Net + VAT;
}
