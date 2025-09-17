using System;
using System.Collections.Generic;
using Ergonaut.Core.Models;
using Ergonaut.Core.Utilities;

namespace Ergonaut.Sentinel.Services
{
    /// <summary>
    /// Service responsible for generating intelligent tasks based on various inputs and patterns.
    /// </summary>
    public interface ITaskGenerator
    {
        void GenerateTask(string context);
    }

    public class TaskGenerator : ITaskGenerator
    {
        public void GenerateTask(string context) { }
    }

    /// <summary>
    /// Defines the types of recurring patterns for task generation.
    /// </summary>
    public enum RecurrencePattern
    {
        Daily,
        Weekly,
        Monthly,
    }
}
