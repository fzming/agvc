using System.Collections.Generic;

namespace AgvcWorkFactory.Tasks
{
    /// <summary>
    /// 用户自定义任务
    /// </summary>
    public class UserTask:ITask
    {
        /// <summary>
        ///  任务ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 指定MRID
        /// </summary>
        public string MRID { get; set; }
        /// <summary>
        /// Froms 定义
        /// </summary>
        public List<TaskGoal> FromGoals { get; set; }

        /// <summary>
        /// Tos 定义
        /// </summary>
        public List<TaskGoal> ToGoals { get; set; }

        /// <summary>
        /// 优先处理
        /// </summary>
        public bool  Priority { get; set; }
    }
}