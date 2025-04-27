using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;

namespace ToDoList.Service.Interfaces
{
    public interface ITaskService 
    {
        Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model); 
        Task<IBaseResponse<IEnumerable<TaskViewModel>>> GetTasks(TaskFilter taskFilter);
    }
}
