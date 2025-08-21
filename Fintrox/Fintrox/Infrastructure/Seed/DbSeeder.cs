using Fintrox.Domain.Accounting;
using Fintrox.Domain.Partners;
using Microsoft.EntityFrameworkCore;

namespace Fintrox.Infrastructure.Seed;

public static class DbSeeder
{
	public static async Task SeedAsync(AppDbContext db)
	{
		await db.Database.MigrateAsync();

		if (!await db.Accounts.AnyAsync())
		{
			// Сметкоплан (минимален)
			var a1000 = new Account { Id = Guid.NewGuid(), Code = "1000", Name = "Активи" };
			var a1010 = new Account { Id = Guid.NewGuid(), Code = "1010", Name = "Каса", Parent = a1000 };
			var a1020 = new Account { Id = Guid.NewGuid(), Code = "1020", Name = "Банка", Parent = a1000 };

			var a4000 = new Account { Id = Guid.NewGuid(), Code = "4000", Name = "Приходи" };
			var a6000 = new Account { Id = Guid.NewGuid(), Code = "6000", Name = "Разходи" };

			db.Accounts.AddRange(a1000, a1010, a1020, a4000, a6000);

			// Контрагент
			db.Counterparties.Add(new Counterparty
			{
				Id = Guid.NewGuid(),
				Name = "Demo Ltd.",
				Bulstat = "1234567890"
			});

			await db.SaveChangesAsync();
		}
	}
}
