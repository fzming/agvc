using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AgvUtility;
using Protocol;
using Protocol.Mission;
using Protocol.Query;

using Serialize;
using MRStatus = RobotDefine.MRStatus;

namespace RobotFactory
{
    public class WS
    {
        private const string header = "http://localhost:1025/IMServer/Dispatch?json=";
        /// <summary>
        /// 執行Mission,Query,Interrupt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static T Dispatch<T>(Base query) where T : class
        {
            var json = query.SerializeJSONObject();
            using (WebClient client = new WebClient())
            {
                try
                {
                    byte[] bytes = client.DownloadData(header + json);
                    char[] trimChars = new char[] { '"' };
                    string str = Encoding.ASCII.GetString(bytes).Trim(trimChars).Replace("\\\"", "\"");
                    return str.DeserializeJsonToObject() as T;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    return default;
                }

            }

        }
        /// <summary>
        ///  異步執行Mission,Query,Interrupt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<T> DispatchAsync<T>(Base query) where T : class
        {
            var json = query.SerializeJSONObject();
            using (WebClient client = new WebClient())
            {
                try
                {
                    byte[] bytes = await client.DownloadDataTaskAsync(header + json);
                    char[] trimChars = new char[] { '"' };
                    string str = Encoding.ASCII.GetString(bytes).Trim(trimChars).Replace("\\\"", "\"");
                    return str.DeserializeJsonToObject() as T;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    return default;
                }
            }

        }

        public static MRStatus GetMRStatus(string mrid)
        {
            var response = Dispatch<Protocol.Query.MRStatus.Response>(new Protocol.Query.MRStatus
            {
                MRID = mrid
            });
            return response?.MRStatus;
        }
        /// <summary>
        /// 當機器人身上無貨物時，命令機器人前往充電
        /// </summary>
        /// <param name="mrid"></param>
        /// <returns></returns>
        public static bool SendDockMission(string mrid)
        {
            var mission = new Dock {MRID = mrid, MissionID = Guid.NewGuid().ToString("N")};
            var response =  AsyncHelper.RunSync(() =>
            {
                return DispatchAsync<Protocol.Mission.Dock.Response>(mission);
            });
            if (response!=null)
            {
                return response.Accept;
            }
            return false;
        }
    }
}