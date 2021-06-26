using System.Threading.Tasks;
using AgvcCoreData.Users;
using AgvcEntitys.Users;
using CoreData;
using CoreRepository;

namespace AgvcRepository.Users.Interfaces
{
    /// <summary>
    ///     用户收件箱仓储接口
    /// </summary>
    public interface IUserLetterBoxRepository : IRepository<UserLetterBox>
    {
        /// <summary>
        ///     用户收件箱高级查询
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<PageResult<UserLetterBox>> AdvanceQueryUserLetterBoxAsync(string clientId, LetterBoxPageQuery condition);

        Task<bool> UserReadAllAsync(string clientId);
    }
}