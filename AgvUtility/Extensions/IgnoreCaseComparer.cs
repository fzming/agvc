using System;
using System.Collections.Generic;

namespace Utility.Extensions
{
    /// <summary>
    /// 忽略大小写比较器,作为Contains方法的参数
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class IgnoreCaseComparer : IEqualityComparer<string>
    {
        /// <inheritdoc />
        public int GetHashCode(string t)
        {
            return t.GetHashCode();
        }
        /// <summary>
        /// 重写它的Equals，保存同时重写GetHashCode
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(string x, string y)
        {
            return string.Equals((x ?? string.Empty)
                .Trim(), (y ?? string.Empty)
                .Trim(), StringComparison.CurrentCultureIgnoreCase);
        }

    }
}
