using System.Threading.Tasks;
using Protocol;
using RobotDefine;
using Utility;

namespace AgvcWorkFactory
{
    public interface IWS : ISingletonDependency
    {
        /// <summary>
        ///     執行Mission,Query,Interrupt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        T Dispatch<T>(Base query) where T : class;

        /// <summary>
        ///     異步執行Mission,Query,Interrupt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<T> DispatchAsync<T>(Base query) where T : class;

        /// <summary>
        ///     取得線上機器人狀態
        /// </summary>
        /// <param name="mrid"></param>
        /// <returns></returns>
        MRStatus GetMRStatus(string mrid);

        /// <summary>
        ///     當機器人身上無貨物時，命令機器人前往充電
        /// </summary>
        /// <param name="mrid"></param>
        /// <returns></returns>
        bool SendDockMission(string mrid);
    }
}