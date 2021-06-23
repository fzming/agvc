using System;

namespace RobotFactory.Tasks
{
    public class TaskTypeAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Attribute" /> class.</summary>
        public TaskTypeAttribute(RobotTaskType taskType)
        {
            TaskType = taskType;
        }

        public RobotTaskType TaskType { get; }
    }
}