using System;

namespace Protocol.Report
{
    public class TimeoutAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Attribute" /> class.</summary>
        public TimeoutAttribute(double milliseconds)
        {
            Milliseconds = milliseconds;
        }

        /// <summary>
        ///     毫秒数
        /// </summary>
        public double Milliseconds { get; set; }
    }
}