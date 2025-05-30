using Atlas.Components;
using Atlas.Data;
using Atlas.Interfaces;
using Atlas.Repositories;
using Atlas.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddRazorComponents()
    .AddInteractiveServerComponents();

services.AddScoped<ITelegramService, TelegramService>();
services.AddScoped<INotificationService, TelegramNotificationService>();
services.AddScoped<ITelegramChatRepository, TelegramChatRepository>();

services.AddHttpClient();

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

var telegramService = app.Services.CreateScope().ServiceProvider.GetRequiredService<ITelegramService>();
await telegramService.StartReceivingAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();