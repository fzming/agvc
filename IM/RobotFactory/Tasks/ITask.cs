using System.Collections.Generic;
using Utility;

namespace AgvcWorkFactory.Tasks
{
    public interface ITask : ITransientDependency
    {
        /// <summary>
        ///     任务ID
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     MRID
        /// </summary>
        public string MRID { get; set; }

        /// <summary>
        ///     Froms
        /// </summary>
        List<TaskGoal> Froms { get; set; }

        /// <summary>
        ///     Tos
        /// </summary>
        List<TaskGoal> Tos { get; set; }

    }
}