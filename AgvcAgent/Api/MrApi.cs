using System.Threading.Tasks;
using AgvcRepository;
using AgvcRepository.Entitys;
using Microsoft.AspNetCore.Mvc;

namespace AgvcAgent.Api
{
    [ApiController]
    [Route("mr")]
    public class MrApi:ControllerBase
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