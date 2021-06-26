using System;
using System.Threading;

namespace Utility.Helpers
{
    public static class OrderNoGenerator
    {
        private static readonly object _locker = new();
        private static int _sn;
        public static string GenerateId(string prefix)
        {
            lock (_locker) //lock 关键字可确保当一个线程位于代码的临界区时，另一个线程不会进入该临界区。
            {
                if (_sn == int.MaxValue)
                {
                    _sn = 0;
                }
                else
                {
                    _sn++;
                }

                Thread.Sleep(100);

                return prefix + DateTime.Now.ToString("yyyyMMddHHmmss") + _sn.ToString().PadLeft(4, '0');
            }
        }
    }
}