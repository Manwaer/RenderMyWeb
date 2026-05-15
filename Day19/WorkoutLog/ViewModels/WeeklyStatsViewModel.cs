namespace WorkoutLog.ViewModels
{
    public class WeeklyStatsViewModel
    {
        public DateTime StartOfWeek { get; set; }
        public DateTime EndOfWeek { get; set; }
        public int WeekOffset { get; set; } // Для переключения недель (-1 - прошлая, 0 - текущая)

        public int TotalWorkouts { get; set; }
        public int TotalDuration { get; set; }
        public int MaxDailyDuration { get; set; } // Для масштабирования столбцов графика

        // Статистика по дням недели (Пн, Вт, Ср...) -> Минуты
        public Dictionary<string, int> DurationByDay { get; set; } = new();

        // Статистика по типам тренировок (Бег, Плавание...) -> Количество
        public Dictionary<string, int> WorkoutsByType { get; set; } = new();

        public string TotalDurationFormatted =>
            $"{TotalDuration / 60} ч {TotalDuration % 60} мин";
    }
}