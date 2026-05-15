using System;
using System.ComponentModel.DataAnnotations;

namespace WorkoutLog.Models
{
    public class Workout
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Дата обязательна")]
        [Display(Name = "Дата тренировки")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Тип тренировки обязателен")]
        [Display(Name = "Тип тренировки")]
        [StringLength(100, ErrorMessage = "Максимум 100 символов")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Продолжительность обязательна")]
        [Display(Name = "Продолжительность (минуты)")]
        [Range(1, 500, ErrorMessage = "Продолжительность должна быть от 1 до 500 минут")]
        public int Duration { get; set; }

        [Display(Name = "Упражнение")]
        [StringLength(200, ErrorMessage = "Максимум 200 символов")]
        public string? Exercise { get; set; }

        [Display(Name = "Время начала")]
        [DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }

        [Display(Name = "Заметки")]
        [StringLength(500, ErrorMessage = "Максимум 500 символов")]
        public string? Notes { get; set; }
    }
}