using LibraryManagement.Configuration;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.Services.ZarinPal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LibraryDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<LibraryDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookCopyService, BookCopyService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IdentityErrorLocalizer>();
builder.Services.AddHttpClient<IZarinPalService, ZarinpalService>();
builder.Services.AddScoped<IPaymentAttemptService, PaymentAttemptService>();
builder.Services.Configure<ZarinPalOptions>(builder.Configuration.GetSection("ZarinPal"));


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<LibraryDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

    await SeedData.Initialize(context, userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
