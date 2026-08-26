using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var konekcioniString = builder.Configuration.GetConnectionString("KonekcioniString");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();          // required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);    // how long session lives
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;                 // needed if you use GDPR consent
});

/*// ===== COOKIES (optional – already available, but you can configure defaults) =====
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;      // only if you want cookie consent
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});
*/

builder.Services.AddHttpClient("TakmicenjeAPIKlijent", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!); 
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pocetna}/{action=Index}")
    .WithStaticAssets();


app.Run();
