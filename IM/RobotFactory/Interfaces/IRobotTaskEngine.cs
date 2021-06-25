using System;
using AgvcWorkFactory.Tasks;
using Messages.Transfers.Core;
using Utility;

namespace AgvcWorkFactory.Interfaces
{
    public interface IRobotTaskEngine : IDisposable,ISingletonDependency
    {
        /// <summary>
        ///  Accept MES消息 Transfer Request(MES->AGVC)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="MRID">指定MR接受任务</param>
        void AcceptMessage(IMessage message, string MRID);

        /// <summary>
        /// Accept 用户任务指令
        /// </summary>
        void AcceptUserTask(ITask userTask);

        void Start();
        void Stop();
    }
}