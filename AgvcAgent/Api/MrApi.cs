using System.Threading.Tasks;
using AgvcAgent.Api.Kernel;
using AgvcEntitys.Agv;
using AgvcRepository;
using Microsoft.AspNetCore.Mvc;

namespace AgvcAgent.Api
{
    [ApiController]
    [Route("mr")]
    public class MrApi : AuthorizedApiController
    {
        public MrApi(IMrRepository mrRepository)
        {
            MrRepository = mrRepository;
        }

        private IMrRepository MrRepository { get; }

        /// <summary>
        ///     获取MR实体
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