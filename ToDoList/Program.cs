


using Microsoft.EntityFrameworkCore;
using ToDoList.DAL;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

var connetionString = builder.Configuration.GetConnectionString("PgSql");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connetionString);
});

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
    pattern: "{controller=Task}/{action=Index}/{id?}");

app.Run();
