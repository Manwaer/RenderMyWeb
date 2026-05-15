using WorkoutLog.Models;
using WorkoutLog.ViewModels;

namespace WorkoutLog.Services
{
    public interface IWorkoutService
    {
        // Получить все тренировки
        Task<IEnumerable<Workout>> GetAllAsync();

        // Получить тренировки по типу
        Task<IEnumerable<Workout>> GetByTypeAsync(string type);

        // Получить тренировки за конкретный день
        Task<IEnumerable<Workout>> GetByDateAsync(DateTime date);

        // Получить тренировку по ID
        Task<Workout?> GetByIdAsync(int id);

        // Создать тренировку из ViewModel
        Task<Workout> CreateAsync(WorkoutViewModel viewModel);

        // Обновить тренировку из ViewModel
        Task<bool> UpdateAsync(WorkoutViewModel viewModel);

        // Удалить тренировку
        Task<bool> DeleteAsync(int id);

        // Проверить существование тренировки
        Task<bool> ExistsAsync(int id);

        // Получить все доступные типы тренировок из БД
        Task<IEnumerable<string>> GetAllTypesAsync();

        // Получить статистику за период
        Task<WorkoutStatsViewModel> GetStatsAsync(DateTime from, DateTime to);
        Task<WeeklyStatsViewModel> GetWeeklyStatsAsync(int weekOffset = 0);
    }
}