using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using CoreData;
using CoreService;
using Utility.Extensions;

namespace AgvcService.Users
{
    [Export(typeof(IUserLetterBoxService))]
    internal class UserLetterBoxService : AbstractService, IUserLetterBoxService
    {
        private IUserLetterBoxRepository Repository { get; }

        [ImportingConstructor]
        public UserLetterBoxService(IUserLetterBoxRepository repository)
        {
            Repository = repository;
        }

        public async Task<bool> SendUserLetterBoxAsync(string clientId, LetterBox letter, bool read = false)
        {
            try
            {
                var entity = letter.MapTo(new UserLetterBox());
                entity.UserId = clientId;
                entity.Read = read;
                await Repository.InsertAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public Task<PageResult<UserLetterBox>> QueryUserLetterBoxAsync(string clientId, LetterBoxPageQuery query)
        {
            return Repository.AdvanceQueryUserLetterBoxAsync(clientId, query);
        }

        public Task<bool> DeleteUserLetterAsync(string clientId, string id)
        {
            return Repository.DeleteAsync(p => p.UserId == clientId && p.Id == id);
        }

        public Task<long> GetUserLetterUnreadCountAsync(string clientId)
        {
            return Repository.CountAsync(p => p.UserId == clientId && p.Read == false);
        }

        public Task<bool> UserReadAllAsync(string clientId)
        {
            return Repository.UserReadAllAsync(clientId);
        }
    }
}