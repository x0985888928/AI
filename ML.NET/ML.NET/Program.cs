using Microsoft.EntityFrameworkCore;
using ML.NET.Data;
using ML.NET.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 向 DI 容器註冊 AppDbContext，並指定連線字串
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 設定 Serilog 日誌紀錄
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // 設定最低日誌等級（Debug, Information, Warning, Error）
    .WriteTo.Console()    // 寫入 Console
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day) // 寫入檔案，並以每日分割
    .CreateLogger();

// 將 Serilog 設定成 Host 的日誌提供者
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();


var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
