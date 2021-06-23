using System.Collections.Generic;

namespace RobotFactory.Tasks
{
    public interface ITask
    {
        /// <summary>
        ///  任务ID
        /// </summary>
        string Id { get; }
        /// <summary>
        /// MRID
        /// </summary>
        public string MRID { get; set; }

        /// <summary>
        /// Froms
        /// </summary>
        List<TaskGoal> FromGoals { get; set; }
        /// <summary>
        /// Tos
        /// </summary>
        List<TaskGoal> ToGoals { get; set; }
    }
}