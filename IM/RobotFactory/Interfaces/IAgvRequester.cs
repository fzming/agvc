using Protocol;
using Protocol.Request;
using Utility;

namespace AgvcWorkFactory.Interfaces
{
    public interface IAgvRequester : ISingletonDependency
    {
        bool TryAddWatch(AgvRequest agvRequest);
        void RemoveWatch(string key);
        int GetAgvInitializeInterval();
        /// <summary>
        ///     IM->AGVC  请求了状态
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        BaseRequest.Response OnRequest(BaseRequest request);

        BaseRequest GetRequest(string key);
    }
}