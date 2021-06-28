using Utility;

namespace AgvcWorkFactory.Interfaces
{
    public class AgvcConfig
    {
        /// <summary>
        /// 服务器监听地址
        /// </summary>
        public string ListenUrls { get; set; }
        /// <summary>
        /// AGVC开放给IM的接口地址
        /// </summary>
        public string Report { get; set; }
        /// <summary>
        /// IM WebServiceDispatch URI地址
        /// </summary>
        public string IMUrl { get; set; }
        /// <summary>
        /// 充电策略
        /// </summary>
        public ChargePolicy ChargePolicy { get; set; }
        /// <summary>
        /// 任務引擎分配任务間隔毫秒
        /// </summary>
        public int TaskAssignMs { get; set; }
        /// <summary>
        /// 机器人执行任务间隔毫秒数
        /// </summary>
        public int RobotWorkInterval { get; set; }
        /// <summary>
        /// 当机器人有待处理的任务，但忙碌或充电时，轮询检查间隔MS
        /// </summary>
        public int RobotIdleStatusInterval { get; set; }
        /// <summary>
        /// 在等待Agv的Report和Request过程中，轮询检查Agv是否被重置的间隔MS 
        /// </summary>
        public int InitializeCheckInterval { get; set; }
    }

    /// <summary>
    /// 充电策略
    /// </summary>
    public class ChargePolicy
    {
        /// <summary>
        /// 电量小于(30%)时自动充电
        /// </summary>
        public double Battery { get; set; }
        /// <summary>
        /// 如果充电前电量小于(10%)
        /// </summary>
        public double IfDockBattery { get; set; }
        /// <summary>
        /// 至少充电至(70%)
        /// </summary>
        public double StillBattery { get; set; }
    }


    public interface IAgvcConfiguration:ISingletonDependency
    {
        AgvcConfig GetConfig();
    }
}