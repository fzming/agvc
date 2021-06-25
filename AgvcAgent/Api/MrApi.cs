using System.Threading.Tasks;
using AgvcAgent.Api.Kernel;
using AgvcRepository;
using AgvcRepository.Entitys;
using Microsoft.AspNetCore.Mvc;
namespace AgvcAgent.Api
{
    [ApiController]
    [Route("mr")]
    public class MrApi : AuthorizedApiController
    {
        private IMrRepository MrRepository { get; }

        public MrApi(IMrRepository mrRepository)
        {
            MrRepository = mrRepository;
        }
        /// <summary>
        /// 获取MR实体
        /// </summary>
        /// <param name="mrId"></param>
        /// <returns></returns>
        [Route("get")]
        public Task<MrEntity> Get(string mrId)
        {
            return MrRepository.FirstAsync(p => p.MrId == mrId);
        }
    }
}