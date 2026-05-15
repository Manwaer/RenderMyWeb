using Microsoft.AspNetCore.Mvc;
using WorkoutLog.Services;
using WorkoutLog.ViewModels;

namespace WorkoutLog.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly IWorkoutService _workoutService;

        // DI: внедрение сервиса через конструктор
        public WorkoutsController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        // GET: /Workouts?filterType=Бег
        public async Task<IActionResult> Index(string? filterType)
        {
            IEnumerable<WorkoutLog.Models.Workout> workouts;

            if (!string.IsNullOrWhiteSpace(filterType))
            {
                // Фильтрация по типу через сервис
                workouts = await _workoutService.GetByTypeAsync(filterType);
                ViewBag.CurrentFilter = filterType;
            }
            else
            {
                workouts = await _workoutService.GetAllAsync();
                ViewBag.CurrentFilter = string.Empty;
            }

            // Передаём доступные типы для выпадающего списка
            ViewBag.AvailableTypes = await _workoutService.GetAllTypesAsync();

            return View(workouts);
        }

        // GET: /Workouts/Day/2024-01-15
        [Route("Workouts/Day/{date}")]
        public async Task<IActionResult> Day(string date)
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate))
                return BadRequest("Неверный формат даты");

            var workouts = await _workoutService.GetByDateAsync(parsedDate);
            ViewBag.SelectedDate = parsedDate;

            return View(workouts);
        }

        // GET: /Workouts/Create
        public IActionResult Create()
        {
            var viewModel = new WorkoutViewModel
            {
                Date = DateTime.Today
            };
            return View(viewModel);
        }

        // POST: /Workouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _workoutService.CreateAsync(viewModel);

            // Сообщение об успехе через TempData
            TempData["SuccessMessage"] = $"Тренировка \"{viewModel.Type}\" успешно добавлена!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Workouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var workout = await _workoutService.GetByIdAsync(id.Value);
            if (workout == null)
                return NotFound();

            // Конвертируем модель в ViewModel
            var viewModel = WorkoutViewModel.FromModel(workout);
            return View(viewModel);
        }

        // POST: /Workouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(viewModel);

            var updated = await _workoutService.UpdateAsync(viewModel);
            if (!updated)
                return NotFound();

            // Сообщение об успехе через TempData
            TempData["SuccessMessage"] = $"Тренировка \"{viewModel.Type}\" успешно обновлена!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Workouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var workout = await _workoutService.GetByIdAsync(id.Value);
            if (workout == null)
                return NotFound();

            return View(workout);
        }

        // POST: /Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workout = await _workoutService.GetByIdAsync(id);
            var typeName = workout?.Type ?? "Тренировка";

            await _workoutService.DeleteAsync(id);

            // Сообщение через TempData
            TempData["SuccessMessage"] = $"\"{typeName}\" успешно удалена!";

            return RedirectToAction(nameof(Index));
        }
        // GET: /Workouts/WeeklyStats?weekOffset=0
        public async Task<IActionResult> WeeklyStats(int weekOffset = 0)
        {
            var stats = await _workoutService.GetWeeklyStatsAsync(weekOffset);
            return View(stats);
        }
    }
}