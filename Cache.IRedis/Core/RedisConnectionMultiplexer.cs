using System;
using Cache.IRedis.Interfaces;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Cache.IRedis.Core
{
    /// <summary>
    /// Redis连接池
    /// ConnectionMultiplexer对象是StackExchange.Redis最中枢的对象。
    /// 这个类的实例需要被整个应用程序域共享和重用的
    /// 所以不需要在每个操作中不停的创建该对象的实例，一般都是使用单例来创建和存放这个对象
    /// </summary>

    public class RedisConnectionMultiplexer : IRedisConnectionMultiplexer
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public RedisConnectionMultiplexer(IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration;
            Logger = logger;
            var section = configuration.GetSection("Redis");
            this.RedisConfig = section.Get<RedisConfig>();
            Init();
        }

        private IConfiguration Configuration { get; }
        private ILogger Logger { get; set; }

        private static readonly object locker = new object();
        public ConnectionMultiplexer ConnectionMultiplexer { get; set; }

        public  RedisConfig RedisConfig { get; set; }

        private Lazy<IServer> LazyServer { get; set; }
        public IServer Server => LazyServer.Value;
      

        private void Init()
        {

            /*
             *配置选项 https://blog.csdn.net/wulex/article/details/78394725
            ConfigurationOptions 包含大量的配置选项，一些常用的配置如下：

            abortConnect ： 当为true时，当没有可用的服务器时则不会创建一个连接
            allowAdmin ： 当为true时 ，可以使用一些被认为危险的命令
            channelPrefix：所有pub/sub渠道的前缀
            connectRetry ：重试连接的次数
            connectTimeout：超时时间
            configChannel： Broadcast channel name for communicating configuration changes
            defaultDatabase ： 默认0到-1
            keepAlive ： 保存x秒的活动连接
            name:ClientName
            password:password
            proxy:代理 比如 twemproxy
            resolveDns : 指定dns解析
            serviceName ： Not currently implemented (intended for use with sentinel)
            ssl={bool} ： 使用sll加密
            sslHost={string}	： 强制服务器使用特定的ssl标识
            syncTimeout={int} ： 异步超时时间
            tiebreaker={string}：Key to use for selecting a server in an ambiguous master scenario
            version={string} ： Redis version level (useful when the server does not make this available)
            writeBuffer={int} ： 输出缓存区的大小

             */
            var option = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                //KeepAlive = 180,
                //链接点
                EndPoints = { RedisConfig.RedisServer },
                Password = RedisConfig.RedisPassword,
                DefaultDatabase = RedisConfig.RedisDefaultDatabase,
                //  PreserveAsyncOrder = false
            };
            LazyServer = new Lazy<IServer>(() =>
            {
                var endpoints = ConnectionMultiplexer.GetEndPoints(); //列出节点
                return ConnectionMultiplexer.GetServer(endpoints[0]);
            });
            ConnectionMultiplexer = GetConnectionMultiplexer(option);

        }

        /// <summary>
        /// 创建Redis链接
        /// </summary>
        /// <param name="options">连接配置</param>
        /// <returns></returns>
        private ConnectionMultiplexer GetConnectionMultiplexer(ConfigurationOptions options = null)
        {
            var connect = ConnectionMultiplexer.Connect(options);

            #region 注册事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;
            #endregion

            Logger.LogDebug("ConnectionMultiplexer.Connect", options);
            return connect;
        }

        #region Redis事件
        /// <summary>
        /// 内部异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Logger.LogError("内部异常：" + e.Exception.Message);
        }

        /// <summary>
        /// 集群更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Logger.LogError("新集群：" + e.NewEndPoint + "旧集群：" + e.OldEndPoint);
        }

        /// <summary>
        /// 配置更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Logger.LogError("配置更改：" + e.EndPoint);
        }

        /// <summary>
        /// 错误事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Logger.LogError("异常信息：" + e.Message);
        }

        /// <summary>
        /// 重连错误事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Logger.LogError("重连错误" + e.EndPoint);
        }

        /// <summary>
        /// 连接失败事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Logger.LogError("连接异常" + e.EndPoint + "，类型为" + e.FailureType + (e.Exception == null ? "" : ("，异常信息是" + e.Exception.Message)));
        }
        #endregion


    }
}
