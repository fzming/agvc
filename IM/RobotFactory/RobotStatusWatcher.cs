using System;
using System.Collections.Generic;
using System.Threading;
using AgvcWorkFactory.Interfaces;

namespace AgvcWorkFactory
{
    /// <summary>
    ///     MR状态监控
    /// </summary>
    public class RobotStatusWatcher : IDisposable, IRobotStatusWatcher
    {
        private IWS Ws { get; }

        /// <summary>
        ///     线程取消句柄
        /// </summary>
        private CancellationTokenSource _cancelTokenSource;

        /// <summary>
        ///     原子信号量
        /// </summary>
        private readonly AutoResetEvent _waitHandle = new(false);

        /// <summary>
        ///     监控线程
        /// </summary>
        private Thread _watchThread;

        /// <summary>
        ///     监控MR队列
        /// </summary>
        private readonly Queue<string> queue = new();

        /// <summary>
        ///     同步锁
        /// </summary>
        private readonly object syncRoot = new();

        #region IDisposable

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public RobotStatusWatcher(IWS ws)
        {
            Ws = ws;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Stop();
        }

        #endregion

        /// <summary>
        ///     当接收到MR状态改变事件
        /// </summary>
        public event MrStatusReceivedEventHandler MrStatusReceived;

        /// <summary>
        ///     当MR状态无法成功获取事件
        /// </summary>
        public event MrStatusErrorEventHandler MrStatusError;

        /// <summary>
        ///     将MR加入到工作队列
        /// </summary>
        /// <param name="MRID"></param>
        public void Watch(string MRID)
        {
            lock (syncRoot)
            {
                if (queue.Contains(MRID)) return;
                if (_watchThread == null)
                {
                    _cancelTokenSource = new CancellationTokenSource();
                    _watchThread = new Thread(QueueWatchThread) {IsBackground = true};
                    _watchThread.Start();
                }

                queue.Enqueue(MRID);
                _waitHandle.Set(); //发送信号
            }
        }

        /// <summary>
        ///     停止线程操作
        /// </summary>
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

        /// <summary>
        ///     状态更新执行线程
        /// </summary>
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
                        var mrStatus = Ws.GetMRStatus(mrid);
                        if (mrStatus != null)
                            MrStatusReceived?.Invoke(this, new MrStatusEventArg
                            {
                                MrStatus = mrStatus
                            });
                        else
                            MrStatusError?.Invoke(this, new MrStatusErrorArg
                            {
                                MRID = mrid,
                                Error = "GetMRStatus Network Error"
                            });
                    }
                }

                if (!string.IsNullOrEmpty(mrid))
                    Thread.Sleep(100); //继续执行下一个队列项
                else //队列中没有数据
                    _waitHandle.WaitOne(); //阻塞线程
            }
        }
    }
}