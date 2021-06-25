using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.System;
using AgvcEntitys.Users;
using AgvcRepository.System.Interfaces;
using AgvcRepository.Users.Interfaces;
using CoreData;
using CoreRepository;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AgvcRepository.Users
{
    public class DiscussMessageRepository : MongoRepository<DiscussMessage>, IDiscussMessageRepository
    {
        #region IOC


        private IAccountRepository AccountRepository { get; }
        private ISystemUserRepository SystemUserRepository { get; }
        public DiscussMessageRepository(IMongoUnitOfWork unitOfWork,IAccountRepository accountRepository,ISystemUserRepository systemUserRepository) : base(unitOfWork)
        {
            
            AccountRepository = accountRepository;
            SystemUserRepository = systemUserRepository;
        }

        #endregion

        public async Task<bool> SetFlagAsync(string messageId, string userId, MessageFlag flag)
        {
            //确保只有一个线程进入
           
                var filter = Filter.And(
                    Filter.Eq(x => x.Id, messageId),
                    Filter.ElemMatch(x => x.UserFlags, x => x.Id == userId));

                var update = Updater
                    .Set(p => p.UserFlags.ElementAt(-1).Id, userId)
                    .Set(p => p.UserFlags.ElementAt(-1).Time, DateTime.Now)
                    .Set(p => p.UserFlags.ElementAt(-1).Flag, flag);
                var rs = await Collection.UpdateOneAsync(filter,
                    update.CurrentDate(i => i.ModifiedOn));
                var modifiedCount = rs.ModifiedCount;
                if (modifiedCount < 1)
                {
                    update = Updater
                        .AddToSet(p => p.UserFlags, new UserMsgFlag
                        {
                            Id = userId,
                            Time = DateTime.Now,
                            Flag = flag
                        });
                    rs = await Collection.UpdateOneAsync(Filter.Eq(x => x.Id, messageId),
                        update.CurrentDate(i => i.ModifiedOn));
                    modifiedCount = rs.ModifiedCount;
                }

                return modifiedCount > 0;
            
        }

        public Task<PageResult<DiscussMessageModel>> QueryGroupMessagesAsync(string orgId, DiscussMessagePageQuery query)
        {
            var q = Collection.AsQueryable().Where(p =>
                p.OrgId == orgId && p.Group == query.Group);
            q = query.OnlyNew ? q.Where(p => p.UserFlags.Any(k => k.Id == query.UserId) == false) : q.Where(p => p.UserFlags.Any(k => k.Id == query.UserId && k.Flag == MessageFlag.已删) == false);

            if (query.Group.StartsWith("$")) //系统用户组消息
            {
                var joinQuery = q.Join(SystemUserRepository.DynamicCollection as IMongoCollection<SystemUser>,
                    d => d.SenderId, a => a.Id, (dis, user) => new { dis, user }).Select(p => new DiscussMessageModel
                {
                    Id = p.dis.Id,
                    Sender = new IdentityUser
                    {
                        Id = p.user.Id,
                        Name = p.user.Nick,
                        Avatar = p.user.Avatar
                    },
                    Group = p.dis.Group,
                    Content = p.dis.Content,
                    Attachment = p.dis.Attachment,
                    CreatedOn = p.dis.CreatedOn,
                    IsRead = p.dis.UserFlags.Any(c => c.Id == query.UserId)
                });
                return joinQuery.ToPageListAsync(query, p => p.CreatedOn, true);
            }
            else
            {
               var  joinQuery = q.Join(AccountRepository.DynamicCollection as IMongoCollection<Account>,
                    d => d.SenderId, a => a.Id, (dis, user) => new { dis, user }).Select(p => new DiscussMessageModel
                {
                    Id = p.dis.Id,
                    Sender = new IdentityUser
                    {
                        Id = p.user.Id,
                        Name = p.user.Nick,
                        Avatar = p.user.Avatar
                    },
                    Group = p.dis.Group,
                    Content = p.dis.Content,
                    Attachment = p.dis.Attachment,
                    CreatedOn = p.dis.CreatedOn,
                    IsRead = p.dis.UserFlags.Any(c => c.Id == query.UserId)
                });

               return joinQuery.ToPageListAsync(query, p => p.CreatedOn, true);
            }
            
           
        }

        public async Task<Dictionary<string,int>> GetUnReadMessageCountAsync(string orgId, string clientId,string[] groups)
        {
            var query =  Collection.AsQueryable().Where(p =>
                p.OrgId == orgId && groups.Contains(p.Group)&& p.UserFlags.Any(k => k.Id == clientId) == false)
                .GroupBy(p=>p.Group).Select(p=> new {
                    group = p.Key,
                    count = p.Count()
                });

            return (await query.ToListAsync()).ToDictionary(p => p.group, v => v.count);

        }

        public  async  Task<IdentityUser> GetSenderAsync(string clientId, bool isSys)
        {
            var user =  isSys ? await SystemUserRepository.GetAsync(clientId) :
                (await AccountRepository.GetAsync(clientId)) as UserAccountBase;
            if (user == null)
            {
                return new IdentityUser();
            }
            return new IdentityUser
            {
                Id = user.Id,
                Name = user.Nick,
                Avatar = user.Avatar
            };
        }

    }
}