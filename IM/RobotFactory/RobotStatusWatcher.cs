using System.Collections.Generic;
using System.Threading;

namespace RobotFactory
{
    /// <summary>
    /// MR状态监控
    /// </summary>
    public class RobotStatusWatcher
    {
        public event MrStatusReceivedEventHandler MrStatusReceived;
        private Queue<string> queue = new Queue<string>();
        private static object _locker = new object();
        private CancellationTokenSource _cancelTokenSource;
        private Thread _watchThread;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public RobotStatusWatcher()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _watchThread = new Thread(QueueWatchThread) { IsBackground = true };
            _watchThread.Start();
        }


        public void Stop()
        {
            try
            {
                _cancelTokenSource.Cancel(false);
                _watchThread.Join();
                _watchThread = null;
            }
            catch
            {
                // ignored
            }
        }
        public void Watch(string MRID)
        {
            queue.Enqueue(MRID);
        }
        private void QueueWatchThread()
        {
            while (!_cancelTokenSource.IsCancellationRequested)
            {
                //Console.WriteLine("队列内任务量:" + (TaskQueue.Count));//输出时间 毫秒
                lock (_locker)
                {
                    while (queue.Count > 0)
                    {

                        var mrid = queue.Dequeue();
                        //调用IM WS 更新MR状态

                        var response = WS.Dispatch<Protocol.Query.MRStatus.Response>(new Protocol.Query.MRStatus
                        {
                            MRID = mrid
                        });
                        MrStatusReceived?.Invoke(this, new MrStatusEventArg
                        {
                            MrStatus = response.MRStatus
                        });
                        //
                        // Thread.Sleep(10);
                    }
                }
            }
        }
    }
}