using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Extenstions;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Service.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly IBaseRepository<TaskEntity> _taskRepository;
        private ILogger<TaskService> _logger;
        public TaskService(IBaseRepository<TaskEntity> taskRepository, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }
        public async Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model)
        {
            try
            {
                model.Validate();

                _logger.LogInformation($"Запрос на создании задачи - {model.Name}");
                var today = DateTime.UtcNow.Date;
                var tomorrow = today.AddDays(1);

                var task = await _taskRepository.GetAll()
                    .Where(x => x.Created >= today && x.Created < tomorrow)
                    .FirstOrDefaultAsync(x => x.Name == model.Name);

                if (task != null) {
                    return new BaseResponse<TaskEntity>()
                    {
                        Description = "Задача с таким названием уже есть",
                        StatusCode = StatusCode.TaskIsHasAlready
                    };
                }
                task = new TaskEntity()
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsDone = false,
                    Priority = model.Priority,
                    Created = DateTime.UtcNow,  // Используй UTC время
                };

                await _taskRepository.Create(task);

                _logger.LogInformation($"Задача создалась: {task.Name} {task.Created} ");
                return new BaseResponse<TaskEntity>()
                {
                    Description = "Задача создалась",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[TaskService.Create]: {ex.Message}");
                return new BaseResponse<TaskEntity>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> GetTasks(TaskFilter filter)
        {
            try
            {
                var tasks = await _taskRepository.GetAll()
                    .WhereIf(!string.IsNullOrWhiteSpace(filter.Name), x => x.Name == filter.Name)
                    .WhereIf(filter.Priority.HasValue, x => x.Priority == filter.Priority)
                    .Select(x => new TaskViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        IsDone = x.IsDone ? "Готова" : "Не готова",
                        Priority = x.Priority.GetDisplayName(),
                        Created = x.Created.ToLongDateString()
                    })
                    .ToListAsync();

                return new BaseResponse<IEnumerable<TaskViewModel>>()
                {
                    Data = tasks,
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"[TaskService.GetTasks]: {ex.Message}");
                return new BaseResponse<IEnumerable<TaskViewModel>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
