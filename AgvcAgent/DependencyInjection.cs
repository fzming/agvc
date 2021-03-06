using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Utility;

namespace AgvcAgent
{
    public static class DependencyInjection
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void ConfigureServices(WebHostBuilderContext webHostBuilderContext, IServiceCollection services)
        {
            services.ScanAndInjectService("^AgvcWorkFactory.dll|^Utility|^Messages.dll");
            services.ScanAndInjectService("^AgvcService.dll|^CoreService.dll");
            services.ScanAndInjectService("^AgvcRepository.dll|^CoreRepository.dll");
            services.ScanAndInjectService("^Cache.IRedis.dll");
        }


        /// <summary>
        ///     ServiceCollection批量注入方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="matchAssemblies">要扫描的程序集名称,默认为[^Shop.Utils|^Shop.]多个使用|分隔</param>
        /// <returns></returns>
        internal static IServiceCollection ScanAndInjectService(this IServiceCollection services,
            string matchAssemblies)
        {
            bool Match(string assemblyName)
            {
                assemblyName = Path.GetFileName(assemblyName);
                if (assemblyName.StartsWith($"{AppDomain.CurrentDomain.FriendlyName}.Views"))
                    return false;
                if (assemblyName.StartsWith($"{AppDomain.CurrentDomain.FriendlyName}.PrecompiledViews"))
                    return false;
                return Regex.IsMatch(assemblyName, matchAssemblies, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            #region 依赖注入

            var baseType = typeof(IDependency);
            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(path);
            var getFiles = Directory.GetFiles(path, "*.dll").Where(Match);
            var referencedAssemblies =
                getFiles.Select(Assembly.LoadFrom).ToList();

            var ss = referencedAssemblies.SelectMany(o => o.GetTypes());

            var types = referencedAssemblies
                .SelectMany(a => a.DefinedTypes)
                .Select(type => type.AsType())
                .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToList();
            var implementTypes = types.Where(x => x.IsClass & !x.IsAbstract).ToList();
            var interfaceTypes = types.Where(x => x.IsInterface).ToList();
            foreach (var implementType in implementTypes)
                if (typeof(IScopeDependency).IsAssignableFrom(implementType))
                {
                    var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                    if (interfaceType != null)
                    {
                        Console.WriteLine($"[AddScope] {interfaceType.Name}>{implementType.Name}");
                        services.AddScoped(interfaceType, implementType);
                    }
                }
                else if (typeof(ISingletonDependency).IsAssignableFrom(implementType))
                {
                    var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                    if (interfaceType != null)
                    {
                        Console.WriteLine($"[AddSingleton] {interfaceType.Name}>{implementType.Name}");
                        services.AddSingleton(interfaceType, implementType);
                    }
                }
                else
                {
                    var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                    if (interfaceType != null)
                    {
                        Console.WriteLine($"[AddTransient] {interfaceType.Name}>{implementType.Name}");
                        services.AddTransient(interfaceType, implementType);
                    }
                }

            #endregion

            return services;
        }

        public static T GetService<T>() where T : class
        {
            return (T)ServiceProvider.GetService(typeof(T));
        }
    }
}