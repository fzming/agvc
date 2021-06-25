using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Utility
{
    /// <summary>
    /// 路径转换辅助类
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// 本地路径转换成URL相对路径
        /// </summary>
        /// <param name="absolutepath">绝对路径</param>
        /// <param name="appfullpath">程序根目录</param>
        /// <param name="relroot">相对根目录</param>
        /// <returns>返回URL相对路径</returns>
        public static string ConvertToRelativePath(string absolutepath,string appfullpath,string relroot="")
        { 
            //绝对路径转换成相对路径
            var relativeroot = absolutepath.ToLower().Replace(appfullpath.ToLower(), "");
            relativeroot = relativeroot.Replace(@"\", @"/");
            return relroot+relativeroot;
        }
        /// <summary>
        /// 相对路径转换成服务器本地物理路径
        /// </summary>
        /// <param name="relativepath"></param>
        /// <returns></returns>
        public static string ConvertToAbsolutePath(string relativepath)
        {
          
            return MapPath(relativepath); //转换成绝对路径
        }
        /// <summary>
        /// 组合路径
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            if (paths.Length == 0)
            {
                throw new ArgumentException("please input path");
            }

            var builder = new StringBuilder();
            var spliter = "\\";

            var firstPath = paths[0];

            if (firstPath.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
            {
                spliter = "/";
            }

            if (!firstPath.EndsWith(spliter))
            {
                firstPath = firstPath + spliter;
            }
            builder.Append(firstPath);

            for (var i = 1; i < paths.Length; i++)
            {
                var nextPath = paths[i];
                if (nextPath.StartsWith("/") || nextPath.StartsWith("\\"))
                {
                    nextPath = nextPath.Substring(1);
                }

                if (i != paths.Length - 1)//not the last one
                {
                    if (nextPath.EndsWith("/") || nextPath.EndsWith("\\"))
                    {
                        nextPath = nextPath.Substring(0, nextPath.Length - 1) + spliter;
                    }
                    else
                    {
                        nextPath = nextPath + spliter;
                    }
                }

                builder.Append(nextPath);
            }

            return builder.ToString().Replace("/","\\");
        }

        /// <summary>
        /// 多线程下MapPath
        /// </summary>
        /// <param name="strPath">相对路径</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static string MapPath(string strPath)
        {
            if (string.IsNullOrEmpty(strPath))
            {
                return strPath;
            }
            if (strPath.IndexOf(":", StringComparison.Ordinal) != -1) //已是绝对路径
            {
                return strPath;
            }

            if (strPath.StartsWith("../")) //使用父目录的相对路径转绝对路径 注意，只支持../开头
            {
           
                var reg = new Regex(@"\.\./");
                var count =reg.Matches(strPath).Count;
                var rootDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);//当前根目录
    
                for (var i = 0; i < count; i++)
                {
                    rootDir = rootDir?.Parent;
                }

                var lp = reg.Replace(strPath, "").Replace("/", "\\");

                return Path.Combine(rootDir?.FullName ?? throw new InvalidOperationException(), lp);
            }
            //strPath = strPath.Replace("\\", "/");
            strPath = strPath.Replace("~", "");
            if (strPath.StartsWith("//"))
            {
                strPath = strPath.TrimStart('/').TrimStart('/').Replace('/','\\');
            }
            return Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }

        /// <summary>
        /// 是否是物理路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsPhysicsPath(this string path)
        {
            return Regex.IsMatch(path, @"\b[a-z]:\\.*", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
        }
     
     
    }
}
