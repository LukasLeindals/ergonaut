using System;
using System.Collections.Generic;
using System.Linq;
using Ergonaut.Core.Models;
using Ergonaut.Core.Utilities;

namespace Ergonaut.Sentinel.Repositories
{
    /// <summary>
    /// Repository for managing TaskItem entities.
    /// </summary>
    public interface ITaskRepository
    {
        void SaveTask();
    }

    public class TaskRepository : ITaskRepository
    {
        public void SaveTask() { }
    }
}
