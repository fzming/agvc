using System.Net;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using Protocol.Query;
using Serialize;

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
                byte[] bytes = client.DownloadData(header + json);
                char[] trimChars = new char[] { '"' };
                string str = Encoding.ASCII.GetString(bytes).Trim(trimChars).Replace("\\\"", "\"");
                return str.DeserializeJsonToObject() as T;
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
                byte[] bytes = await client.DownloadDataTaskAsync(header + json);
                char[] trimChars = new char[] { '"' };
                string str = Encoding.ASCII.GetString(bytes).Trim(trimChars).Replace("\\\"", "\"");
                return str.DeserializeJsonToObject() as T;
            }

        }
    }
}