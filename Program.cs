<<<<<<< HEAD
//using learnify.Models;
//using Learnify.Data;
using learnify.Models;
=======
using Learnify.Data;
>>>>>>> d8a853457f552bd777265d92154d33039dd0f266
using Learnify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

<<<<<<< HEAD
builder.Services.AddDbContext<AppDbContenxt>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
=======
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
>>>>>>> d8a853457f552bd777265d92154d33039dd0f266

builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedAccount = false;
})
<<<<<<< HEAD
    .AddEntityFrameworkStores<AppDbContenxt>()
=======
    .AddEntityFrameworkStores<ApplicationDbContext>()
>>>>>>> d8a853457f552bd777265d92154d33039dd0f266
    .AddDefaultTokenProviders();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();