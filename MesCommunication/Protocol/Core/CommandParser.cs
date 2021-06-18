using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MesCommunication.Protocol
{
    class CommandMap
    {
        public Type Type { get; set; }
        public List<CommandProperty> CommandProperties { get; set; }
    }
    class CommandProperty
    {
        public int Length { get; set; }
        public string Name { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public int Order { get; set; }
    }


    public static class CommandParser
    {
        static Dictionary<string, CommandMap> CommandPropsCache = new();

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        static CommandParser()
        {
            ReflectionCommandMap();
        }
        /// <summary>
        /// 反射获取所有Command类型映射
        /// </summary>
        private static void ReflectionCommandMap()
        {
            var types = Assembly.GetAssembly(typeof(Command)).GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && t.IsAbstract == false);
            foreach (var type in types)
            {
                var key = type.Name.ToUpper();
                var propertys = type.GetProperties().Select(p => new CommandProperty
                {
                    Name = p.Name,
                    PropertyInfo = p,
                    Order = p.GetCustomAttribute<DeserializationAttribute>()?.Order ?? 0,
                    Length = p.GetCustomAttribute<DeserializationAttribute>()?.Length ?? 0
                }).OrderBy(o => o.Order).ToList();
                CommandPropsCache.Add(key, new CommandMap()
                {
                    Type = type,
                    CommandProperties = propertys
                });
            }
        }

        public static ICommand Parse(string message)
        {
            var header = message.Substring(0, 6).ToUpper();
            if (CommandPropsCache.TryGetValue(header, out var commandMap))
            {
                //开始解析Command
                var instance = Activator.CreateInstance(commandMap.Type) as ICommand;
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
    }
}