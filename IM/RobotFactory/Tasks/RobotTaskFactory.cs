using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Utility.Extensions;

namespace AgvcWorkFactory.Tasks
{
    public class RobotTaskFactory : IRobotTaskFactory
    {
        private IServiceProvider ServiceProvider { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public RobotTaskFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IRobotTask CreateRobotTask(TaskPathType pathType)
        {
            var taskType = pathType.GetAttribute<TaskTypeAttribute>().TaskType;
            var taskServices = ServiceProvider.GetServices<IRobotTask>();
            var task = taskServices.FirstOrDefault(p => p.TaskType == taskType);
            if (task != null)
            {
                task.PathType = pathType;
                return task;
            }
            return null;
        }
    }
}