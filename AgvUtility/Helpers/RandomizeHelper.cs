using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Extensions;

namespace Utility.Helpers
{
    /// <summary>
    /// 随机数字或字符生成工具类
    /// </summary>
    public static class RandomizeHelper
    {
        private static int _rep;


        /// <summary>
        /// 生成随机数字字符串
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        /// <returns>生成的数字字符串</returns>
        public static string GenerateNumbers(int codeCount)
        {
            var str = string.Empty;
            var num2 = DateTime.Now.Ticks + _rep;
            _rep++;
            var random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> _rep)));
            for (var i = 0; i < codeCount; i++)
            {
                var num = random.Next();
                str = str + ((char)(0x30 + ((ushort)(num % 10))));
            }

            return str;
        }


        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        /// <returns>生成的字母字符串</returns>
        public static string GenerateNumberAndWords(int codeCount)
        {
            var str = string.Empty;
            var num2 = DateTime.Now.Ticks + _rep;
            _rep++;
            var random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> _rep)));

            for (var i = 0; i < codeCount; i++)
            {
                char ch;
                var num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }

                str += ch;
            }

            return str;
        }

        public static Random CreateGuidRandom()
        {
            var buffer = Guid.NewGuid().ToByteArray();
            var iSeed = BitConverter.ToInt32(buffer, 0);
            var r = new Random(iSeed);
            return r;
        }
        /// <summary>
        /// 生成纯字母随机数
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        /// <returns></returns>
        public static string GenerateWords(int codeCount)
        {

            char[] pattern =
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z'
            };
            var result = "";
            var n = pattern.Length;
            var random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (var i = 0; i < codeCount; i++)
            {
                var rnd = random.Next(0, n);
                result += pattern[rnd];
            }

            return result;

        }

        public static string GenerateSecurityKey()
        {
            const int charCodeA = 'A';
            var words = new StringBuilder();
            var timestamp = DateTime.Now.ToJsTimestamp().ToString(); //毫秒级时间戳
            //
            // if (length == 0 || length > timestamp.Length)
            // {
            //
            //     length = timestamp.Length;
            //
            // }
            var queue = new Queue<int>();
            var timestamps = timestamp.ToCharArray().Select(p => p.ToString()).ToList();
            timestamps.Reverse();

            foreach (var s in timestamps.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                queue.Enqueue(int.Parse(s));
            }

            var endprex = CreateGuidRandom().Next(1, 10);

            while (queue.Any())
            {
                var n = CreateGuidRandom().Next(1, 26); //20
                var word = ((char)(n + charCodeA)).ToString(); //20+65 U
                var ts = ((char)(queue.Dequeue() + charCodeA)).ToString(); // ? + 65
                var ns = n + endprex; //20+9
                words.Append(ns + "" + word + "" + ts); //29 U ?
            }
            return words.ToString() + endprex;
        }
    }
}
