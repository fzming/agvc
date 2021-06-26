using Utility;

namespace AgvcWorkFactory.Tasks
{
    public interface IRobotTaskFactory:ISingletonDependency
    {
        IRobotTask CreateRobotTask(TaskPathType pathType);
    }
}