using Microsoft.EntityFrameworkCore;
using WorkoutLog.Data;
using WorkoutLog.Models;
using WorkoutLog.ViewModels;

namespace WorkoutLog.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получить все тренировки, отсортированные по дате
        public async Task<IEnumerable<Workout>> GetAllAsync()
        {
            return await _context.Workouts
                .OrderByDescending(w => w.Date)
                .ThenBy(w => w.StartTime)
                .ToListAsync();
        }

        // Фильтр по типу тренировки (без учёта регистра)
        public async Task<IEnumerable<Workout>> GetByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return await GetAllAsync();

            return await _context.Workouts
                .Where(w => w.Type.ToLower().Contains(type.ToLower()))
                .OrderByDescending(w => w.Date)
                .ToListAsync();
        }

        // Фильтр по дате
        public async Task<IEnumerable<Workout>> GetByDateAsync(DateTime date)
        {
            return await _context.Workouts
                .Where(w => w.Date.Date == date.Date)
                .OrderBy(w => w.StartTime)
                .ThenBy(w => w.Type)
                .ToListAsync();
        }

        // Получить тренировку по ID
        public async Task<Workout?> GetByIdAsync(int id)
        {
            return await _context.Workouts.FindAsync(id);
        }

        // Создать новую тренировку
        public async Task<Workout> CreateAsync(WorkoutViewModel viewModel)
        {
            var workout = viewModel.ToModel();
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return workout;
        }

        // Обновить существующую тренировку
        public async Task<bool> UpdateAsync(WorkoutViewModel viewModel)
        {
            var existing = await _context.Workouts.FindAsync(viewModel.Id);
            if (existing == null)
                return false;

            // Обновляем поля
            existing.Date = viewModel.Date;
            existing.Type = viewModel.Type;
            existing.Exercise = viewModel.Exercise;
            existing.Duration = viewModel.Duration;
            existing.StartTime = viewModel.StartTime;
            existing.Notes = viewModel.Notes;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(viewModel.Id))
                    return false;
                throw;
            }
        }

        // Удалить тренировку
        public async Task<bool> DeleteAsync(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
                return false;

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return true;
        }

        // Проверить существование
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Workouts.AnyAsync(w => w.Id == id);
        }

        // Получить уникальные типы тренировок из БД
        public async Task<IEnumerable<string>> GetAllTypesAsync()
        {
            return await _context.Workouts
                .Select(w => w.Type)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
        }

        // Статистика за период
        public async Task<WorkoutStatsViewModel> GetStatsAsync(DateTime from, DateTime to)
        {
            var workouts = await _context.Workouts
                .Where(w => w.Date.Date >= from.Date && w.Date.Date <= to.Date)
                .ToListAsync();

            return new WorkoutStatsViewModel
            {
                TotalWorkouts = workouts.Count,
                TotalDuration = workouts.Sum(w => w.Duration),
                AverageDuration = workouts.Any()
                    ? (int)workouts.Average(w => w.Duration)
                    : 0,
                MostPopularType = workouts
                    .GroupBy(w => w.Type)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key ?? "Нет данных",
                WorkoutsByType = workouts
                    .GroupBy(w => w.Type)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
        // Добавьте этот метод в класс WorkoutService
        public async Task<WeeklyStatsViewModel> GetWeeklyStatsAsync(int weekOffset = 0)
        {
            // 1. Определение границ недели (Понедельник - Воскресенье)
            var today = DateTime.Today.AddDays(weekOffset * 7);
            // Вычисляем понедельник текущей недели
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = today.AddDays(-1 * diff).Date;
            var endOfWeek = startOfWeek.AddDays(6).Date;

            // 2. Запрос к БД
            var workouts = await _context.Workouts
                .Where(w => w.Date.Date >= startOfWeek && w.Date.Date <= endOfWeek)
                .ToListAsync();

            // 3. Подготовка структуры для графика по дням (чтобы пустые дни тоже отображались)
            var daysMap = new Dictionary<string, DateTime>
    {
        { "Пн", startOfWeek },
        { "Вт", startOfWeek.AddDays(1) },
        { "Ср", startOfWeek.AddDays(2) },
        { "Чт", startOfWeek.AddDays(3) },
        { "Пт", startOfWeek.AddDays(4) },
        { "Сб", startOfWeek.AddDays(5) },
        { "Вс", startOfWeek.AddDays(6) }
    };

            var durationByDay = new Dictionary<string, int>();
            foreach (var day in daysMap)
            {
                // Суммируем длительность тренировок для конкретного дня
                durationByDay[day.Key] = workouts
                    .Where(w => w.Date.Date == day.Value)
                    .Sum(w => w.Duration);
            }

            // 4. Группировка по типам
            var workoutsByType = workouts
                .GroupBy(w => w.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            // 5. Возврат ViewModel
            return new WeeklyStatsViewModel
            {
                StartOfWeek = startOfWeek,
                EndOfWeek = endOfWeek,
                WeekOffset = weekOffset,
                TotalWorkouts = workouts.Count,
                TotalDuration = workouts.Sum(w => w.Duration),
                MaxDailyDuration = durationByDay.Values.DefaultIfEmpty(0).Max(),
                DurationByDay = durationByDay,
                WorkoutsByType = workoutsByType
            };
        }
    }
}