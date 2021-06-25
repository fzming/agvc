using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using AgvcService.Users.Models;
using CoreData;
using CoreService;
using Microsoft.AspNetCore.Identity;
using Utility.Extensions;

namespace AgvcService.Users
{
    /// <summary>
    /// 讨论组消息服务实现
    /// </summary>
    [Export(typeof(IDiscussMessageService))]
    internal class DiscussMessageService : AbstractService, IDiscussMessageService
    {
        #region IOC

        private IDiscussMessageRepository DiscussMessageRepository { get; }
        private IUploadService UploadService { get; }
        private ISignalrService SignalrService { get; }

        [ImportingConstructor]
        public DiscussMessageService(IDiscussMessageRepository discussMessageRepository,
            IUploadService uploadService,
            ISignalrService signalrService)
        {
            DiscussMessageRepository = discussMessageRepository;
            UploadService = uploadService;
            SignalrService = signalrService;
        }

        #endregion

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="orgIds"></param>
        /// <param name="senderOrgId"></param>
        /// <param name="senderId"></param>
        /// <param name="discussMessageModel"></param>
        /// <returns></returns>
        public async Task<List<DiscussMessage>> SendAsync(List<string> orgIds, string senderOrgId, string senderId,
            SendDiscussMessageModel discussMessageModel)
        {
            var readFlags = new[]
            {
                new UserMsgFlag
                {
                    Id = senderId,
                    Flag = MessageFlag.已读,
                    Time = DateTime.Now
                }
            };
            var messages = orgIds.Select(p => discussMessageModel.MapTo(new DiscussMessage
            {
                SenderId = senderId,
                OrgId = p,
                UserFlags = senderOrgId == p ? readFlags : new UserMsgFlag[] { }
            })).ToList();
            await DiscussMessageRepository.InsertAsync(messages);
            return messages;
        }

        public async Task<DiscussMessage> SendAsync(string orgId, string senderId, SendDiscussMessageModel discussMessageModel)
        {
            var message = new DiscussMessage
            {
                SenderId = senderId,
                OrgId = orgId,
                UserFlags = new[]
                {
                    new UserMsgFlag
                    {
                        Id = senderId,
                        Flag = MessageFlag.已读,
                        Time = DateTime.Now
                    }
                }
            };

            discussMessageModel.MapTo(message);
            await DiscussMessageRepository.InsertAsync(message);
            return message;
        }

        /// <summary>
        /// 设为已读或已删除
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="userId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public async Task<bool> SetFlagAsync(string messageId, string userId, MessageFlag flag)
        {
            using (await _mutex.LockAsync())
            {
                return await DiscussMessageRepository.SetFlagAsync(messageId, userId, flag);
            }
        }

        /// <summary>
        /// 物理删除整条消息
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(string messageId)
        {
            return DiscussMessageRepository.DeleteAsync(messageId);
        }

        /// <summary>
        /// 获取讨论组消息列表
        /// </summary>
        /// <param name="orgId">机构ID</param>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        public Task<PageResult<DiscussMessageModel>> QueryGroupMessagesAsync(string orgId, DiscussMessagePageQuery query)
        {
            return DiscussMessageRepository.QueryGroupMessagesAsync(orgId, query);
        }

        /// <summary>
        /// 获取发送人员
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="isSys"></param>
        /// <returns></returns>
        public Task<IdentityUser> GetSenderAsync(string clientId, bool isSys)
        {
            return DiscussMessageRepository.GetSenderAsync(clientId, isSys);
        }

        public Task<bool> BroadCastMessageToAsync(string orgId, string senderId, DiscussMessageModel dto)
        {

            //广播消息
            return SignalrService.BroadcastMessageAsync(new DiscussMessageBroadCast
            {
                Title = "讨论组附件消息",
                Message = dto
            }, orgId, senderId);
        }

        public async Task<List<DiscussMessage>> PostAttachmentMessageAsync(List<string> receiveOrgIds,
            string senderOrgId, string senderId,
            PostDiscussAttachmentModel postDiscussAttachmentModel)
        {
            var attachmentUrl =
                await UploadService.UploadFileAsync(postDiscussAttachmentModel.File, postDiscussAttachmentModel.Option);

            var messages = receiveOrgIds.Select(p => postDiscussAttachmentModel.MapTo(new DiscussMessage
            {
                Attachment = attachmentUrl,
                SenderId = senderId,
                OrgId = p,
                UserFlags = senderOrgId == p ? new[]
                {
                    new UserMsgFlag
                    {
                        Id = senderId,
                        Flag = MessageFlag.已读,
                        Time = DateTime.Now
                    }
                } : new UserMsgFlag[] { }
            })).ToList();
            await DiscussMessageRepository.InsertAsync(messages);
            return messages;
        }

        public async Task<DiscussMessage> PostAttachmentMessageAsync(string orgId, string senderId, PostDiscussAttachmentModel postDiscussAttachmentModel)
        {
            var attachmentUrl = await UploadService.UploadFileAsync(postDiscussAttachmentModel.File, null);
            var message = postDiscussAttachmentModel.MapTo(new DiscussMessage
            {
                Attachment = attachmentUrl,
                SenderId = senderId,
                OrgId = orgId,
                UserFlags = new[]
                {
                    new UserMsgFlag
                    {
                        Id = senderId,
                        Flag = MessageFlag.已读,
                        Time = DateTime.Now
                    }
                }
            });
            await DiscussMessageRepository.InsertAsync(message);
            return message;
        }

        public Task<Dictionary<string, int>> GetUnReadMessageCountAsync(string orgId, string clientId, string[] groups)
        {
            return DiscussMessageRepository.GetUnReadMessageCountAsync(orgId, clientId, groups);
        }
    }
}