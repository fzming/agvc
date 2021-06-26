using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Messages.Transfers.Core;
using Utility.Extensions;

namespace Messages.Serializer
{
    public  class MessageSerializer : IMessageSerializer
    {
        private static Dictionary<string, MessageMap> _p;

        static Dictionary<string, MessageMap> MessagePropsCache
        {
            get
            {
                return _p ??= ReflectionMessageMap();
            }
        }

        /// <summary>
        /// 反射获取所有Command类型映射
        /// </summary>
        private static Dictionary<string, MessageMap> ReflectionMessageMap()
        {
            var dics = new Dictionary<string, MessageMap>();
            var types = Assembly.GetAssembly(typeof(MessageBase)).GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IMessage)) && t.IsAbstract == false);
            foreach (var type in types)
            {
                var key = type.Name.ToUpper();
                var propertys = type.GetProperties().Select(p => new MessageProperty
                {
                    Name = p.Name,
                    PropertyInfo = p,
                    Order = p.GetCustomAttribute<DeserializationAttribute>()?.Order ?? 0,
                    Length = p.GetCustomAttribute<DeserializationAttribute>()?.Length ?? 0
                }).OrderBy(o => o.Order).ToList();
                dics.Add(key, new MessageMap()
                {
                    Type = type,
                    CommandProperties = propertys
                });
            }

            return dics;
        }
        /// <summary>
        /// 反序列化来自MES消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IMessage Deserialize(string message)
        {
            var header = message.Substring(0, 6).ToUpper();
            if (MessagePropsCache.TryGetValue(header, out var commandMap))
            {
                //开始解析Command
                var instance = Activator.CreateInstance(commandMap.Type) as IMessage;
                var i = 0;
                foreach (var commandProperty in commandMap.CommandProperties)
                {

                    var value = message.Substring(i, commandProperty.Length);
                    commandProperty.PropertyInfo.SetValue(instance, value);
                    i += commandProperty.Length;

                }

                return instance;
            }

            throw new Exception($"message from head ({header}) can't convert to command");
        }
        /// <summary>
        /// 序列化消息 用于回传MES
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string Serialize(IMessage message)
        {
            var header = message.trx_id.ToUpper();
            if (MessagePropsCache.TryGetValue(header, out var commandMap))
            {
                return commandMap.CommandProperties.Select(x => x.PropertyInfo.GetValue(message).ToString().PadRight(x.Length)).JoinToString("");
            }
            return string.Empty;
        }
    }
}