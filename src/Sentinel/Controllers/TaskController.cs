using System;
using System.Collections.Generic;
using Ergonaut.Core.Models;
using Ergonaut.Core.Utilities;
using Ergonaut.Sentinel.Repositories;
using Ergonaut.Sentinel.Services;

namespace Ergonaut.Sentinel.Controllers
{
    public interface ITaskController
    {
        void DoSomething();
    }

    public class TaskController : ITaskController
    {
        public void DoSomething() { }
    }
}
