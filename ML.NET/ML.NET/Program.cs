using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ML.NET.Data;
using ML.NET.Middlewares;
using ML.NET.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// �V DI �e�����U AppDbContext�A�ë��w�s�u�r��
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddSingleton<PricePredictionService>();

// �[�J Identity �A�ȻP�w�] UI�A�ϥιw�]�� IdentityUser
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // �i�ھڻݨD�վ�ﶵ�A�o�̥��]�w���n�D�q�l�l������
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>();

// �]�w Serilog ��x����
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // �]�w�̧C��x���š]Debug, Information, Warning, Error�^
    .WriteTo.Console()    // �g�J Console
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day) // �g�J�ɮסA�åH�C�����
    .CreateLogger();

// �N Serilog �]�w�� Host ����x���Ѫ�
builder.Host.UseSerilog();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// TEST CI
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
