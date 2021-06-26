using System;
using System.Text;

namespace Utility.Helpers
{
    /// <summary>
    /// Npinyin+微软PinYinConverter（首选Npinyin） 汉字转拼音
    /// </summary>
    public class PingYinHelper
    {

        public static ThreadSafeDictionary<string, string> _firstSpellCache = new();
        public static ThreadSafeDictionary<string, string> _allSpellCache = new();
        /// <summary>
        /// 汉字转全拼
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string GetAllSpell(string strChinese)
        {

            if (strChinese.Length != 0)
            {
                if (_allSpellCache.TryGetValue(strChinese, out var spell))
                {
                    return spell;
                }

                var fullSpell = new StringBuilder();
                foreach (var s in strChinese)
                {
                    fullSpell.Append(GetSpell(s));
                }
                spell = fullSpell.ToString().ToUpper();

                _allSpellCache.Add(strChinese, spell);
                return spell;
            }
            return string.Empty;
        }

        /// <summary>
        /// 汉字转首字母
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string GetFirstSpell(string strChinese)
        {

            try
            {


                if (strChinese.Length != 0)
                {
                    if (_firstSpellCache.TryGetValue(strChinese, out var spell))
                    {
                        return spell;
                    }

                    var fullSpell = new StringBuilder();
                    foreach (var chr in strChinese)
                    {
                        fullSpell.Append(GetSpell(chr)[0]);
                    }

                    spell = fullSpell.ToString().ToUpper();

                    _firstSpellCache.Add(strChinese, spell);

                    return spell;

                }



            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }

        public static string GetSpell(char chr)
        {
            try
            {
                var coverchr = Pinyin.GetPinyin(chr);

                var isChineses = ChineseChar.IsValidChar(coverchr[0]);
                if (isChineses)
                {
                    var chineseChar = new ChineseChar(coverchr[0]);
                    foreach (var value in chineseChar.Pinyins)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            return value.Remove(value.Length - 1, 1);
                        }
                    }
                }

                return coverchr;

            }
            catch (Exception)
            {
                return string.Empty;
            }


        }
    }
}