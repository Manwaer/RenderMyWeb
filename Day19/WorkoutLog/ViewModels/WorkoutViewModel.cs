using System;
using System.ComponentModel.DataAnnotations;
using WorkoutLog.Models;

namespace WorkoutLog.ViewModels
{
    // ViewModel для создания и редактирования тренировки
    public class WorkoutViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Дата обязательна")]
        [Display(Name = "Дата тренировки")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Тип тренировки обязателен")]
        [Display(Name = "Тип тренировки")]
        [StringLength(100, ErrorMessage = "Максимум 100 символов")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Упражнение обязательно")]
        [Display(Name = "Упражнение")]
        [StringLength(200, ErrorMessage = "Максимум 200 символов")]
        public string Exercise { get; set; } = string.Empty;

        [Required(ErrorMessage = "Продолжительность обязательна")]
        [Display(Name = "Продолжительность (минуты)")]
        [Range(1, 500, ErrorMessage = "Продолжительность должна быть от 1 до 500 минут")]
        public int Duration { get; set; }

        [Display(Name = "Время начала")]
        [DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }

        [Display(Name = "Заметки")]
        [StringLength(500, ErrorMessage = "Максимум 500 символов")]
        public string? Notes { get; set; }

        // Список доступных типов тренировок для выпадающего списка
        public List<string> AvailableTypes { get; set; } = new List<string>
        {
            "Бег",
            "Плавание",
            "Велосипед",
            "Тренажерный зал",
            "Йога",
            "Бокс",
            "Футбол",
            "Баскетбол",
            "Другое"
        };

        // Кастомная валидация: дата не может быть в будущем
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date > DateTime.Today)
            {
                yield return new ValidationResult(
                    "Дата тренировки не может быть в будущем",
                    new[] { nameof(Date) }
                );
            }

            if (Date < new DateTime(2000, 1, 1))
            {
                yield return new ValidationResult(
                    "Дата тренировки не может быть раньше 2000 года",
                    new[] { nameof(Date) }
                );
            }
        }

        // Конвертация ViewModel -> Model
        public Workout ToModel()
        {
            return new Workout
            {
                Id = Id,
                Date = Date,
                Type = Type,
                Exercise = Exercise,
                Duration = Duration,
                StartTime = StartTime,
                Notes = Notes
            };
        }

        // Конвертация Model -> ViewModel (статический фабричный метод)
        public static WorkoutViewModel FromModel(Workout workout)
        {
            return new WorkoutViewModel
            {
                Id = workout.Id,
                Date = workout.Date,
                Type = workout.Type,
                Exercise = workout.Exercise ?? string.Empty,
                Duration = workout.Duration,
                StartTime = workout.StartTime,
                Notes = workout.Notes
            };
        }
    }
}