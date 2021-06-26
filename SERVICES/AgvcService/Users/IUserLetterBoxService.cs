using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using CoreData;
using CoreService.Interfaces;

namespace AgvcService.Users
{
    /// <summary>
    ///     用户收件箱服务
    /// </summary>
    public interface IUserLetterBoxService : IService
    {
        /// <summary>
        ///     给用户发信
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="letter"></param>
        /// <param name="read"></param>
        /// <returns></returns>
        Task<bool> SendUserLetterBoxAsync(string clientId, LetterBox letter, bool read = false);

        /// <summary>
        ///     查询收件箱
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<PageResult<UserLetterBox>> QueryUserLetterBoxAsync(string clientId, LetterBoxPageQuery query);

        /// <summary>
        ///     删除信件
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteUserLetterAsync(string clientId, string id);

        /// <summary>
        ///     获取未读消息数量
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<long> GetUserLetterUnreadCountAsync(string clientId);

        /// <summary>
        ///     设定用户所有消息为已读
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<bool> UserReadAllAsync(string clientId);
    }
}