using Microsoft.EntityFrameworkCore;
using PruebaMiguelArias.Data;
using PruebaMiguelArias.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddTransient<PruebaMiguelArias.Services.IEmailService, PruebaMiguelArias.Services.EmailService>();

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)) 
    )
);

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
