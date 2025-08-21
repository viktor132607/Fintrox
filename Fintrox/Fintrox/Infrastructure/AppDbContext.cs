using Fintrox.Data;
using Fintrox.Domain.Accounting;
using Fintrox.Domain.Partners;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fintrox.Infrastructure;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<Account> Accounts => Set<Account>();
	public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
	public DbSet<JournalLine> JournalLines => Set<JournalLine>();
	public DbSet<Counterparty> Counterparties => Set<Counterparty>();

	protected override void OnModelCreating(ModelBuilder b)
	{
		base.OnModelCreating(b);

		b.Entity<Account>()
			.HasIndex(a => a.Code)
			.IsUnique();

		b.Entity<Account>()
			.HasOne(a => a.Parent)
			.WithMany(a => a.Children)
			.HasForeignKey(a => a.ParentId)
			.OnDelete(DeleteBehavior.Restrict);

		b.Entity<JournalLine>()
			.HasOne(l => l.JournalEntry)
			.WithMany(e => e.Lines)
			.HasForeignKey(l => l.JournalEntryId);

		b.Entity<JournalLine>()
			.HasOne(l => l.DebitAccount)
			.WithMany()
			.HasForeignKey(l => l.DebitAccountId)
			.OnDelete(DeleteBehavior.Restrict);

		b.Entity<JournalLine>()
			.HasOne(l => l.CreditAccount)
			.WithMany()
			.HasForeignKey(l => l.CreditAccountId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
