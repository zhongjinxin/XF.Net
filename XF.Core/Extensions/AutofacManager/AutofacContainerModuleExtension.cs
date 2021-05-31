using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using XF.Core.Configuration;
using XF.Core.Extensions.AutofacManager;
using XF.Core.ManagerUser;

namespace XF.Core.Extensions
{
    public static class AutofacContainerModuleExtension
    {
        public static IServiceCollection AddModule(this IServiceCollection services, ContainerBuilder builder, IConfiguration configuration)
        {
            AppSetting.Init(services, configuration);
            Type baseType = typeof(IDependency);
            // 获取所有自定义的类
            var compilationLibrary = DependencyContext.Default
                .CompileLibraries
                .Where(x => !x.Serviceable
                && x.Type == "project")
                .ToList();
            var count1 = compilationLibrary.Count;
            List<Assembly> assemblyList = new List<Assembly>();
            foreach (var _compilation in compilationLibrary)
            {
                try
                {
                    assemblyList.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(_compilation.Name)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(_compilation.Name + ex.Message);
                }
            }
            // 注册
            builder.RegisterAssemblyTypes(assemblyList.ToArray())
            .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract)
            .AsSelf().AsImplementedInterfaces()
            .InstancePerLifetimeScope();
            builder.RegisterType<UserContext>().InstancePerLifetimeScope();


            return services;
        }
    }
}
