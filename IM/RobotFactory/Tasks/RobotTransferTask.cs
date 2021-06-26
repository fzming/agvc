﻿using System;
using Messages.Transfers;
using Messages.Transfers.Core;
using Protocol.Mission;
using Protocol.Report;
using Utility.Extensions;

namespace AgvcWorkFactory.Tasks
{
    /// <summary>
    ///     Robot搬运指令(STK->EQP，EQP->STK,EQP->EQP,STK->STK)
    /// </summary>
    [TaskType(RobotTaskType.Transfer)]
    public class RobotTransferTask : AbstractRobotTask
    {
        /*
         *
         *  EQP1       <==>  STK2
             -PORT1           -PORT1
             -PORT2           -PORT2
            STK1       <==>  EQP2
             -PORT1           -PORT1
             -PORT2           -PORT2
         */

        #region Overrides of RobotTask

        protected override void OnTrxMessageAdded(IMessage message)
        {
            var trx = message as Tx501i;

            FromGoals.Add(new TaskGoal
            {
                BoxID = trx.kemlot_id.Trim(), //货物
                Goal = trx.eqp_from, //目标Stock或EQP 名称
                Port = trx.port_from.ToInt(), //目标Stock或EQP的实际Port 名称
                WaferCount = trx.cur_sublot_wafcnt.ToInt()
            });

            ToGoals.Add(new TaskGoal
                {
                    BoxID = trx.kemlot_id.Trim(), //货物
                    Goal = trx.eqp_to, //目标Stock或EQP 名称
                    Port = trx.port_to.ToInt(), //目标Stock或EQP的实际Port 名称
                    WaferCount = trx.cur_sublot_wafcnt.ToInt()
                }
            );
        }

        /// <summary>
        ///     實際任務单次執行步驟(From)
        /// </summary>
        protected override void OnRunFromTask(TaskGoal goal, int index)
        {
            Console.WriteLine($"{MRID} Begin Pick");
            RunPickMission(new Pick
            {
                BoxID = goal.BoxID,
                Goal = goal.Goal,
                Port = goal.Port,
                WaferCount = goal.WaferCount
            }, report =>
            {
                switch (report)
                {
                    case TransportEnd:
                        //AGV->MES 发送TXD27I(XS)
                        AGVC2MES(new Txd27i());
                        break;
                    case MissionDone:
                        Console.WriteLine($"{MRID} Pick MissionDone");
                        break;
                }
            });
        }

        /// <summary>
        ///     實際任務单次執行步驟(To)
        /// </summary>
        protected override void OnRunToTask(TaskGoal goal, int index)
        {
            Console.WriteLine($"{MRID} Begin Drop");
            RunDropMission(new Drop
                {
                    BoxID = goal.BoxID,
                    Goal = goal.Goal,
                    Port = goal.Port,
                    WaferCount = goal.WaferCount
                },
                report =>
                {
                    switch (report)
                    {
                        case TransportEnd:
                            //AGV->MES 发送TXD27I(XS)
                            AGVC2MES(new Txd27i());
                            break;
                        case MissionDone:
                            Console.WriteLine($"{MRID} Drop MissionDone");
                            break;
                    }
                });
        }

        /// <summary>
        ///     默认执行From To的规则,具体任务实例可以进行重写
        /// </summary>
        public override void ExecuteFromToRules()
        {
            base.ExecuteFromToRules();
        }

        #endregion
    }
}