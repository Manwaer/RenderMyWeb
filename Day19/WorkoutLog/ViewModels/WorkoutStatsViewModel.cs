namespace WorkoutLog.ViewModels
{
    public class WorkoutStatsViewModel
    {
        public int TotalWorkouts { get; set; }
        public int TotalDuration { get; set; }
        public int AverageDuration { get; set; }
        public string MostPopularType { get; set; } = string.Empty;
        public Dictionary<string, int> WorkoutsByType { get; set; } = new();

        // Вычисляемое свойство: часы и минуты
        public string TotalDurationFormatted =>
            $"{TotalDuration / 60} ч {TotalDuration % 60} мин";
    }
}