using System.Collections.Generic;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcService.Users.Models;
using CoreData;
using CoreService.Interfaces;
namespace AgvcService.Users
{
    /// <summary>
    /// 讨论组消息服务
    /// </summary>
    public interface IDiscussMessageService : IService
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="orgIds"></param>
        /// <param name="senderOrgId"></param>
        /// <param name="senderId"></param>
        /// <param name="discussMessageModel"></param>
        /// <returns></returns>
        Task<List<DiscussMessage>> SendAsync(List<string> orgIds, string senderOrgId, string senderId,
            SendDiscussMessageModel discussMessageModel);
        Task<DiscussMessage> SendAsync(string orgId, string senderId, SendDiscussMessageModel discussMessageModel);

        /// <summary>
        /// 设为已读或已删除
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="userId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        Task<bool> SetFlagAsync(string messageId, string userId,MessageFlag flag);
        /// <summary>
        /// 物理删除整条消息
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string messageId);

        /// <summary>
        /// 获取讨论组消息列表
        /// </summary>
        /// <param name="orgId">机构ID</param>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        Task<PageResult<DiscussMessageModel>> QueryGroupMessagesAsync(string orgId, DiscussMessagePageQuery query);

        /// <summary>
        /// 获取未读消息数量
        /// </summary>
        /// <param name="orgId">机构ID</param>
        /// <param name="clientId">用户ID</param>
        /// <param name="groups">消息分组</param>
        /// <returns></returns>
        Task<Dictionary<string, int>> GetUnReadMessageCountAsync(string orgId, string clientId, string[] groups);
        /// <summary>
        /// 获取发送人员
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="isSys"></param>
        /// <returns></returns>
        Task<UserIdentity> GetSenderAsync(string clientId, bool isSys);
        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="senderId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> BroadCastMessageToAsync(string orgId,string senderId, DiscussMessageModel dto);

        Task<List<DiscussMessage>> PostAttachmentMessageAsync(List<string> receiveOrgIds, string senderOrgId,
            string senderId,
            PostDiscussAttachmentModel postDiscussAttachmentModel);
       
    }
}