using System;
using System.Collections.Generic;
using System.Linq;
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
        private CancellationTokenSource _cancelTokenSource;
        private Thread _watchThread;
        private object syncRoot = new object();
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);

        public void Stop()
        {
            try
            {
                _cancelTokenSource.Cancel(false);
                _waitHandle.Close();
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
            lock (syncRoot)
            {
                if (queue.Contains(MRID)) return;
                if (_watchThread == null)
                {
                    _cancelTokenSource = new CancellationTokenSource();
                    _watchThread = new Thread(QueueWatchThread) { IsBackground = true };
                    _watchThread.Start();
                }
                queue.Enqueue(MRID);
                _waitHandle.Set(); //发送信号
            }

        }
        private void QueueWatchThread()
        {
            while (!_cancelTokenSource.IsCancellationRequested)
            {

                var mrid = string.Empty;
                lock (syncRoot)
                {
                    if (queue.Count > 0)
                    {
                        mrid = queue.Dequeue();
                        //调用IM WS 更新MR状态
                       // Console.WriteLine($"[StatusWatcher->调用IM WS 更新MR({mrid})状态]");
                        var response = WS.Dispatch<Protocol.Query.MRStatus.Response>(new Protocol.Query.MRStatus
                        {
                            MRID = mrid
                        });
                        if (response!=null)
                        {
                            MrStatusReceived?.Invoke(this, new MrStatusEventArg
                            {
                                MrStatus = response.MRStatus
                            });
                        }
                        
                    }
                }

                if (!string.IsNullOrEmpty(mrid))
                {
                    Thread.Sleep(100);
                }
                else
                {
                    this._waitHandle.WaitOne(); //阻塞线程
                }

            }
        }
    }
}