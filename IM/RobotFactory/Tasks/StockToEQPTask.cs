using AgvUtility;
using Messages.Transfers;
using Protocol.Mission;
using Protocol.Report;

namespace RobotFactory.Tasks
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
            VirtualRobot.RequestUpdateStatusAsync(); 
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
                return true;//ack
            });
            //4.Transfer End
            WaitReport<TransportEnd>(transportEnd =>
            {
                return true;//ack
            });
            //5.Pick MissionDone
            WaitReport<MissionDone>(missionDone =>
            {
                return true;//ack
            });

        }

        

        #endregion
    }
}