using Microsoft.EntityFrameworkCore;
using WorkoutLog.Data;
using WorkoutLog.Services;

var builder = WebApplication.CreateBuilder(args);

// Регистрация DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация сервиса (Scoped = один экземпляр на запрос)
builder.Services.AddScoped<IWorkoutService, WorkoutService>();

// Добавление MVC с поддержкой TempData
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

// Добавление сессий (нужно для TempData через Session)
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();    // Сессии должны быть до авторизации
app.UseAuthorization();

// Маршрут по умолчанию
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Workouts}/{action=Index}/{id?}");

// Маршрут для фильтра по дню
app.MapControllerRoute(
    name: "workoutsByDay",
    pattern: "Workouts/Day/{date}",
    defaults: new { controller = "Workouts", action = "Day" });

app.Run();