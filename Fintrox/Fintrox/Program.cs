using Fintrox.Client.Pages;
using Fintrox.Components;
using Fintrox.Components.Account;
using Fintrox.Data;
using Fintrox.Infrastructure;
using Fintrox.Infrastructure.Seed;
using Fintrox.Application.Accounting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.WebAssembly.Server;

var builder = WebApplication.CreateBuilder(args);

// ---- DB provider: SQL Server локално / PostgreSQL в Render ----
var databaseUrl = builder.Configuration["DATABASE_URL"];
if (!string.IsNullOrWhiteSpace(databaseUrl))
{
	// Render Postgres
	builder.Services.AddDbContext<AppDbContext>(opt =>
		opt.UseNpgsql(ConvertRenderDatabaseUrl(databaseUrl)));

	// фабрика за Blazor компоненти
	builder.Services.AddDbContextFactory<AppDbContext>(opt =>
		opt.UseNpgsql(ConvertRenderDatabaseUrl(databaseUrl)));
}
else
{
	// Локално: SQL Server (localdb) от appsettings.json
	var conn = builder.Configuration.GetConnectionString("DefaultConnection")
			   ?? "Server=(localdb)\\mssqllocaldb;Database=Fintrox;Trusted_Connection=True;MultipleActiveResultSets=true";

	builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(conn));
	builder.Services.AddDbContextFactory<AppDbContext>(opt => opt.UseSqlServer(conn));
}

// ---- Identity + Blazor ----
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
	.AddIdentityCookies();

builder.Services.AddAuthorization();

builder.Services.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents()
	.AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

// Services
builder.Services.AddScoped<IAccountingService, AccountingService>();

var app = builder.Build();

// DB migrate + seed
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
	await DbSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(Fintrox.Client._Imports).Assembly);

// Identity endpoints
app.MapAdditionalIdentityEndpoints();

// Minimal APIs (accounting)
app.MapAccounting();

app.Run();

static string ConvertRenderDatabaseUrl(string url)
{
	// postgres://user:pass@host:port/dbname
	var uri = new Uri(url);
	var userInfo = uri.UserInfo.Split(':');
	var user = userInfo[0];
	var pass = userInfo.Length > 1 ? userInfo[1] : "";

	// Използвай SSL в Render
	return $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={user};Password={pass};Ssl Mode=Require;Trust Server Certificate=true";
}
