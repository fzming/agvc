using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MongoDB.Bson;

namespace Utility.Extensions
{
    /// <summary>
    ///     字符串扩展函数
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Lazy<IEnumerable<Regex>> LzRegex;

        /// <summary>
        ///     构造函数
        /// </summary>
        static StringExtensions()
        {
            LzRegex = new Lazy<IEnumerable<Regex>>(() =>
            {
                return new[]
                {
                    @"<script[^>]*?>.*?</script>",
                    @"<(///s*)?!?((/w+:)?/w+)(/w+(/s*=?/s*(([""'])(//[""'tbnr]|[^/7])*?/7|/w+)|.{0})|/s)*?(///s*)?>",
                    @"([/r/n])[/s]+",
                    @"([\x00-\x08][\x0b-\x0c][\x0e-\x20])",
                    @"&(quot|#34);",
                    @"&(amp|#38);",
                    @"&(lt|#60);",
                    @"&(gt|#62);",
                    @"&(nbsp|#160);",
                    @"&(iexcl|#161);",
                    @"&(cent|#162);",
                    @"&(pound|#163);",
                    @"&(copy|#169);",
                    @"&#(/d+);",
                    @"-->",
                    @"<!--.*/n"
                }.Select(p => new Regex(p, RegexOptions.IgnoreCase | RegexOptions.Multiline));
            });
        }

        public static IEnumerable<Regex> Regexs => LzRegex.Value;

        /// <summary>
        ///     只允许中文，英文，下划线
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsValidNameInput(this string name)
        {
            return Regex.IsMatch(name, @"^[\u4E00-\u9FA5\w]+$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     正则表达式从地址中提取省市县
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ExtraProvCity(this string addr, string separator = "")
        {
            if (addr.IsNullOrEmpty()) return string.Empty;
            var regexObj = new Regex(@"([\w]+)?(省|市|自治区|自治州|县|区)");
            var match = regexObj.Match(addr);
            var list = new List<string>();
            while (match.Success)
            {
                var n = match.Groups[1].Value;
                if (list.Contains(n) == false) list.Add(n);

                match = match.NextMatch();
            }

            return list.JoinToString(separator);
        }

        public static string AddEndSlash(this string s, string slash = "/")
        {
            if (s.IsNullOrEmpty() || s.Trim().Length == 0) return string.Empty;

            return $"{s}{slash}";
        }

        public static string AddStartSlash(this string s, string slash = "/")
        {
            if (s.IsNullOrEmpty() || s.Trim().Length == 0) return string.Empty;

            return $"{slash}{s}";
        }

        /// <summary>
        ///     忽略大小写进行比较
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="target">目标字符串</param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string source, string target)
        {
            return source.ToStringEx().Equals(target, StringComparison.OrdinalIgnoreCase);
        }

        /*/// <summary>
        /// 汉字转拼音首字母
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToFirstSpellString(this string s)
        {
            return PingYinHelper.GetFirstSpell(s);
        }
        public static string ToFirstSpellChar(this string s)
        {
            return PingYinHelper.GetSpell(s.Left(1)[0])[0].ToString().ToUpper();
        }
        /// <summary>
        /// 汉字转拼音
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToSpellString(this string s)
        {
            return PingYinHelper.GetAllSpell(s);
        }*/
        /// <summary>
        ///     判断输入的字符串只包含汉字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsAllChinese(this string input)
        {
            var regex = new Regex("^[\u4e00-\u9fa5]+$");
            return regex.IsMatch(input);
        }

        /// <summary>
        ///     判断输入的字符串只包含英文字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsAllEnglish(this string input)
        {
            var regex = new Regex("^[a-zA-Z]+$");
            return regex.IsMatch(input);
        }

        /// <summary>
        ///     匹配3位或4位区号的电话号码，其中区号可以用小括号括起来，
        ///     也可以不用，区号与本地号间可以用连字号或空格间隔，
        ///     也可以没有间隔
        ///     \(0\d{2}\)[- ]?\d{8}|0\d{2}[- ]?\d{8}|\(0\d{3}\)[- ]?\d{7}|0\d{3}[- ]?\d{7}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsPhone(this string input)
        {
            var pattern =
                "^\\(0\\d{2}\\)[- ]?\\d{8}$|^0\\d{2}[- ]?\\d{8}$|^\\(0\\d{3}\\)[- ]?\\d{7}$|^0\\d{3}[- ]?\\d{7}$";
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>
        ///     32位MD5编码
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns></returns>
        public static string Md532(this string input)
        {
            if (input == null) return null;

            var md5Hash = MD5.Create();

            // 将输入字符串转换为字节数组并计算哈希数据  
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // 创建一个 Stringbuilder 来收集字节并创建字符串  
            var sBuilder = new StringBuilder();

            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串  
            foreach (var t in data) sBuilder.Append(t.ToString("x2"));

            // 返回十六进制字符串  
            return sBuilder.ToString();
        }

        /// <summary>
        ///     16位MD5编码
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns></returns>
        public static string Md516(this string input)
        {
            return input.Md5(16);
        }

        /// <summary>
        ///     指定位数MD5编码
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="length">编码长度(16位或32位)</param>
        /// <returns></returns>
        public static string Md5(this string s, int length)
        {
            if (length != 16 && length != 32) throw new ArgumentException("Length参数无效,只能为16位或32位");
            var md5 = new MD5CryptoServiceProvider();
            var b = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            var strB = new StringBuilder();
            foreach (var t in b)
                strB.Append(t.ToString("x").PadLeft(2, '0'));

            if (length == 16)
                return strB.ToString(8, 16);
            return strB.ToString();
        }

        /// <summary>
        ///     ToBase64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64(this string str)
        {
            var b = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(b);
        }

        /// <summary>
        ///     FromBase64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FromBase64(this string str)
        {
            var b = Convert.FromBase64String(str);
            return Encoding.Default.GetString(b);
        }

        /// <summary>
        ///     兼容Newtonsoft.Json的CamelCase 驼峰转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
                return s;
            var charArray = s.ToCharArray();
            for (var index = 0; index < charArray.Length && (index != 1 || char.IsUpper(charArray[index])); ++index)
            {
                var flag = index + 1 < charArray.Length;
                if ((index > 0) & flag && !char.IsUpper(charArray[index + 1]))
                {
                    if (char.IsSeparator(charArray[index + 1])) charArray[index] = charArray[index].ToLower();
                    break;
                }

                charArray[index] = charArray[index].ToLower();
            }

            return new string(charArray);
        }

        private static char ToLower(this char c)
        {
            c = char.ToLower(c, CultureInfo.InvariantCulture);
            return c;
        }

        /// <summary>
        ///     是否是正确的安全码
        /// </summary>
        /// <param name="securityKey">安全码串：由JS客户端生成。具体算法参考客户端JS函数</param>
        /// <param name="permissibleTimeSeconds">允许时间戳的误差范围，默认160秒误差</param>
        /// <returns></returns>
        public static bool IsValidSecurityKey(this string securityKey, int permissibleTimeSeconds = 160)
        {
            if (securityKey.Length < 3) throw new Exception("安全码串必须大于3位");
            var lsCode = securityKey.Right(1).ToInt();
            if (lsCode < 1) //最后一位必须大于0
                throw new Exception("安全码串最后一位必须大于0");
            var vsKey = securityKey.Left(securityKey.Length - 1);
            var vsArray = new List<string>();
            var nsArray = new List<string>();
            var tsArray = new List<string>(); //timestamp
            const int charCodeA = 'A';

            var regexObj = new Regex(@"([\-\d]+)([A-Z]+)",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            var match = regexObj.Match(vsKey);
            while (match.Success)
            {
                var n = match.Groups[1].Value.ToInt() - lsCode;
                nsArray.Add(((char) (n + charCodeA)).ToString());
                var w = match.Groups[2].Value;

                if (w.Length == 1)
                {
                    vsArray.Add(w);
                }
                else
                {
                    vsArray.Add(w.Left(1));
                    var ts = w.Right(1)[0] - charCodeA;
                    tsArray.Add(ts.ToString());
                }

                match = match.NextMatch();
            }


            tsArray.Reverse();
            var timestamp = long.Parse(tsArray.JoinToString(""));
            var time = timestamp.ToUnixDatetime();
            var ttl = time.Ttl();
            var compared = vsArray.JoinToString("") == nsArray.JoinToString("");
            var permissionMs = ttl <= permissibleTimeSeconds;
            if (!permissionMs) throw new Exception($"安全码串已超过时限({permissibleTimeSeconds}ms)，请重新获取");

            if (!compared) throw new Exception("安全码串不匹配");
            return true;
        }

        /// <summary>
        ///     判断输入的字符串是否是一个合法的手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobile(this string input)
        {
            var mobile = input.Trim();
            if (string.IsNullOrEmpty(mobile)) return false;
            var regex = new Regex(@"^1[\d]{10}$", RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            return regex.IsMatch(mobile);
        }

        /// <summary>
        ///     Creates a byte array from the string, using the
        ///     System.Text.Encoding.Default encoding unless another is specified.
        /// </summary>
        public static byte[] ToByteArray(this string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        /// <summary>
        ///     在指定的字符串中查找手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<string> FindMobiles(this string input)
        {
            var mobiles = new List<string>();
            try
            {
                var regexObj = new Regex(@"(1\d{10})", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
                var matchResult = regexObj.Match(input);
                while (matchResult.Success)
                {
                    mobiles.Add(matchResult.Value);
                    matchResult = matchResult.NextMatch();
                }
            }
            catch (ArgumentException)
            {
                // Syntax error in the regular expression
            }

            return mobiles.Distinct();
        }

        /// <summary>
        ///     集装箱校验码校验(LINQ版)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool VerifyContainerCode(string code)
        {
            return int.Parse(code.Substring(10, 1)) == code.ToUpper().ToCharArray().Take(10).Select((c, i) => new
            {
                idx = (int) ("0123456789A?BCDEFGHIJK?LMNOPQRSTU?VWXYZ".IndexOf(c) * Math.Pow(2, i))
            }).Sum(p => p.idx) % 11 % 10;
        }

        /// <summary>
        ///     是否是车牌号
        /// </summary>
        /// <param name="vehicleNumber"></param>
        /// <returns></returns>
        public static bool IsVehicleNumber(this string vehicleNumber)
        {
            if (vehicleNumber.IsNullOrEmpty()) return false;
            var result = false;
            if (vehicleNumber.Length == 7)
            {
                var express = @"^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1}$";
                result = Regex.IsMatch(vehicleNumber, express);
            }

            return result;
        }

        /// <summary>
        ///     判断输入的字符串只包含数字
        ///     可以匹配整数和浮点数
        ///     ^-?\d+$|^(-?\d+)(\.\d+)?$
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumber(this string input)
        {
            var pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }


        /// <summary>
        ///     匹配非负整数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNotNagtive(this string input)
        {
            var regex = new Regex(@"^\d+$");
            return regex.IsMatch(input);
        }

        /// <summary>
        ///     匹配整数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsUint(this string input)
        {
            var regex = new Regex("^-?[0-9]*[1-9][0-9]*$");
            return regex.IsMatch(input);
        }


        /// <summary>
        ///     判断输入的字符串是否是一个合法的Email地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmail(this string input)
        {
            var pattern =
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>
        ///     判断字符串是否是BSON ObjectId
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsObjectId(this string input)
        {
            return input.IsNotNullOrEmpty() && ObjectId.TryParse(input, out _);
        }

        public static bool IsObjectIdArray(this string[] inputs)
        {
            return inputs.All(p => p.IsObjectId());
        }

        /// <summary>
        ///     从数组中分析出ObjectId
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="objectIds"></param>
        /// <returns></returns>
        public static bool TryParseObjectId(this object[] objects, out ObjectId[] objectIds)
        {
            objectIds = null;
            if (objects.All(p => p.ToStringEx().IsObjectId()))
            {
                objectIds = objects.Select(p => ObjectId.Parse(p.ToString())).ToArray();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     从指定的字符串中分析出ObjectId
        /// </summary>
        /// <param name="source"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool TryExtractObjectIdArray(this string source, out string[] array)
        {
            var arr = source.Split(',');
            var lst = new List<string>();

            if (arr.All(p => p.IsObjectId()) || source.IsObjectId()) lst.AddRange(arr.Where(s => s.IsObjectId()));

            array = lst.ToArray();
            return lst.Count > 0;
        }

        /// <summary>
        ///     判断输入的字符串是否只包含数字和英文字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumAndEnCh(this string input)
        {
            const string pattern = @"^[A-Za-z0-9]+$";
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }


        /// <summary>
        ///     判断输入的字符串是否是一个超链接
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsUrl(this string input)
        {
            const string pattern = @"^[a-zA-Z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$";
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }


        /// <summary>
        ///     判断输入的字符串是否是表示一个IPV4地址
        /// </summary>
        /// <param name="input">被比较的字符串</param>
        /// <returns>是IP地址则为True</returns>
        public static bool IsIPv4(this string input)
        {
            var ps = input.Split('.');
            var regex = new Regex(@"^\d+$");
            foreach (var t in ps)
            {
                if (!regex.IsMatch(t)) return false;
                if (Convert.ToUInt16(t) > 255) return false;
            }

            return true;
        }

        /// <summary>
        ///     判断输入的字符串是否是合法的IPV6 地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIpv6(this string input)
        {
            string pattern;
            var temp = input;
            var strings = temp.Split(':');
            if (strings.Length > 8) return false;
            var count = GetStringCount(input, "::");
            if (count > 1) return false;
            if (count == 0)
            {
                pattern = @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$";

                var regex = new Regex(pattern);
                return regex.IsMatch(input);
            }

            pattern = @"^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$";
            var regex1 = new Regex(pattern);
            return regex1.IsMatch(input);
        }

        /// <summary>
        ///     计算字符串的字符长度，一个汉字字符将被计算为两个字符
        /// </summary>
        /// <param name="input">需要计算的字符串</param>
        /// <returns>返回字符串的长度</returns>
        public static int LengthEx(this string input)
        {
            return Regex.Replace(input, @"[\u4e00-\u9fa5/g]", "aa").Length;
        }

        /// <summary>
        ///     调用Regex中IsMatch函数实现一般的正则表达式匹配
        /// </summary>
        /// <param name="input">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false。</returns>
        public static bool IsMatch(this string input, string pattern)
        {
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>
        ///     从输入字符串中的第一个字符开始，用替换字符串替换指定的正则表达式模式的所有匹配项。
        /// </summary>
        /// <param name="pattern">模式字符串</param>
        /// <param name="input">输入字符串</param>
        /// <param name="replacement">用于替换的字符串</param>
        /// <returns>返回被替换后的结果</returns>
        public static string RegexReplace(this string input, string pattern, string replacement)
        {
            var regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }

        /// <summary>
        ///     在由正则表达式模式定义的位置拆分输入字符串。
        /// </summary>
        /// <param name="pattern">模式字符串</param>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static string[] RegexSplit(this string input, string pattern)
        {
            var regex = new Regex(pattern);
            return regex.Split(input);
        }


        /// <summary>
        ///     判断字符串compare 在 input字符串中出现的次数
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="compare">用于比较的字符串</param>
        /// <returns>字符串compare 在 input字符串中出现的次数</returns>
        private static int GetStringCount(string input, string compare)
        {
            var index = input.IndexOf(compare, StringComparison.Ordinal);
            if (index != -1) return 1 + GetStringCount(input.Substring(index + compare.Length), compare);
            return 0;
        }

        /// <summary>
        ///     是否是Base64字符串
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static bool IsBase64(this string eStr)
        {
            return eStr.Length % 4 == 0 && Regex.IsMatch(eStr, "^[A-Z0-9/+=]*$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     简单保密手机号
        /// </summary>
        /// <param name="mobile">原始手机号</param>
        /// <returns></returns>
        public static string MobileEncode(this string mobile)
        {
            if (mobile.IsNullOrEmpty()) return "";
            if (mobile.IsMobile()) return mobile.Left(3) + "****" + mobile.Right(4);
            return mobile;
        }


        /// <summary>
        ///     简单保密身份证号
        /// </summary>
        /// <param name="identity">原始身份证</param>
        /// <returns></returns>
        public static string IdentityEncode(this string identity)
        {
            if (identity.IsNullOrEmpty()) return "";
            if (identity.IsIdCard()) return identity.Left(5) + "****" + identity.Right(4);
            return identity;
        }

        /// <summary>
        ///     获取正则表达式Group值
        /// </summary>
        /// <returns>The regex value.</returns>
        /// <param name="str">String.</param>
        /// <param name="regex">Regex.</param>
        /// <param name="index">group index.</param>
        public static string GetRegexValue(this string str, string regex, int index)
        {
            try
            {
                var regexObj = new Regex(regex, RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
                var matchResult = regexObj.Match(str);
                if (matchResult.Success) return matchResult.Groups[index].Value;
            }
            catch (Exception)
            {
                // ignored
            }

            return string.Empty;
        }

        /// <summary>
        ///     简化时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string TimeSimplify(this string time)
        {
            if (time.IsNullOrEmpty()) return "";
            if (time.Length - 5 < 0) return time;
            time = time.Substring(2, time.Length - 5);
            return time;
        }

        /// <summary>
        ///     获取JSONP调用的实际内容
        /// </summary>
        /// <returns>JSONP调用结果实际内容</returns>
        /// <param name="str">String.</param>
        public static string JsonPString(this string str)
        {
            var regexObj = new Regex(@"\((\{.*\})\)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            var matchResult = regexObj.Match(str);
            if (matchResult.Success) return matchResult.Groups[1].Value;

            return string.Empty;
        }

        /// <summary>
        ///     是否为社会统一编码
        ///     营业执照编码 由18位英文数字组成
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsCreditCodeNumber(this string input)
        {
            return Regex.IsMatch(input, @"^[\w\d]{18}$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        ///     是否是身份照号码
        /// </summary>
        /// <param name="card">身份证号码</param>
        /// <returns></returns>
        public static bool IsIdCard(this string card)
        {
            return Regex.IsMatch(card, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase);
        }


        /// <summary>
        ///     首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string str)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str);
        }

        public static bool ContainsAny(this string str, IEnumerable<string> array)
        {
            return array.Any(str.Contains);
        }

        public static bool ContainsAny(this string str, params string[] array)
        {
            return array.Any(str.Contains);
        }

        public static bool ContainsAll(this string str, params string[] array)
        {
            return array.All(str.Contains);
        }

        public static string[] SplitStrings(this string str, string[] splitters)
        {
            return str.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitByRegex(this string str, string pattern = @"[\W|_]")
        {
            return Regex.Split(str, pattern, RegexOptions.IgnoreCase).Where(p => p.IsNotNullOrEmpty()).ToArray();
        }

        /// <summary>
        ///     是否是弱密码
        ///     密码必须是长度为8到16位之间的字母和数字组合
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsWeakPassword(this string str)
        {
            return !Regex.IsMatch(str, @"^.*(?=.{8,16})(?=.*\d)(?=.*[a-zA-Z]{1,}).*$", RegexOptions.IgnoreCase);
        }
    }
}