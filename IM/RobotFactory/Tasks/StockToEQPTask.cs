using System.Collections.Generic;
using AgvUtility;
using Messages.Transfers;
using Messages.Transfers.Core;
using Protocol.Mission;
using Protocol.Query;
using Protocol.Report;


namespace RobotFactory
{
    /// <summary>
    /// 执行搬运单条指令(Stock To EQP)
    /// </summary>
    [TaskType(RobotTaskType.Stock2EQP)]
    public class StockToEqpTask:RobotTask
    {
        #region Overrides of RobotTask

        protected override void OnRun()
        {
            var trx = TransferRequestMessage as Tx501i;
            //1.MrStatus
            VirtualRobot.UpdateStatus(); 
            //2.Pick
            SendMission(new Pick
            {
                BoxID = trx.kemlot_id.Trim(),
                Goal = trx.eqp_to,
                Port = trx.port_to.ToInt(),
                WaferCount = trx.cur_sublot_wafcnt.ToInt(),
            });
            //3.Arrived
            WaitReport<Arrived>(arrived =>
            {
                return true;
            });
            //4.Transfer End
            WaitReport<TransportEnd>(transportEnd =>
            {
                return true;
            });
            //5.Pick MissionDone
            WaitReport<MissionDone>(transportEnd =>
            {
                return true;
            });

        }

        

        #endregion
    }
}