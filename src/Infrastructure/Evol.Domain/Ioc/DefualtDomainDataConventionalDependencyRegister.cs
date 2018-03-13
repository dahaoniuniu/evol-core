﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Evol.Common;
using Evol.Domain.Data;
using Evol.Common.IoC;
using Evol.Configuration.IoC;

namespace Evol.Domain.Ioc
{
    /// <summary>
    /// 默认仓储层组件发现和依赖注册器，在系统启动时被调用，注册进Ioc容器；
    /// 组件有：IQueryEntry、IRepository<T>，IRepository<T, TKey>、IService
    /// </summary>
    public class DefualtDomainDataConventionalDependencyRegister : IConventionalDependencyRegister
    {
        public void Register(IIoCManager ioCManager, Assembly assembly)
        {
            var queryInterfaceImpls = FindQueryEntry(assembly);
            queryInterfaceImpls.ForEach(p => ioCManager.AddPerDependency(p.Interface, p.Impl));
            var repositoryInterfaceImpls = FindRepository(assembly);
            repositoryInterfaceImpls.ForEach(p => ioCManager.AddPerDependency(p.Interface, p.Impl));
        }


        private List<InterfaceImplPair> FindQueryEntry(Assembly assembly)
        {
            Func<Type, bool> filter = type => {
                var tInfo = type.GetTypeInfo();
                return tInfo.IsPublic && !tInfo.IsAbstract && tInfo.IsClass && (typeof(IQueryEntry)).GetTypeInfo().IsAssignableFrom(type);
            };
            List<Type> impls;

            try
            {
                impls = assembly.GetTypes().Where(filter).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var interfaceImpls = new List<InterfaceImplPair>();
            if (impls.Count == 0)
                return interfaceImpls;
            impls.ForEach(t =>
            {
                var @interface = t.GetTypeInfo().GetInterfaces().SingleOrDefault(i => i != typeof(IQueryEntry) && (typeof(IQueryEntry)).GetTypeInfo().IsAssignableFrom(i));
                interfaceImpls.Add(new InterfaceImplPair() { Interface = @interface, Impl = t });
            });
            return interfaceImpls;
        }

        private List<InterfaceImplPair> FindRepository(Assembly assembly)
        {
            Func<Type, bool> filter = type =>
            {
                var tInfo = type.GetTypeInfo();
                var flag = tInfo.IsPublic && !tInfo.IsAbstract && tInfo.IsClass && (typeof(IRepository<>)).GetTypeInfo().IsAssignableFrom(type);
                var flag2 = tInfo.IsPublic && !tInfo.IsAbstract && tInfo.IsClass && (typeof(IRepository<,>)).GetTypeInfo().IsAssignableFrom(type);
                return flag && flag2;
            };
            var impls = assembly.GetTypes().Where(filter).ToList();
            var interfaceImpls = new List<InterfaceImplPair>();
            if (impls.Count == 0)
                return interfaceImpls;
            impls.ForEach(t =>
            {
                var @interface = t.GetTypeInfo().GetInterfaces().SingleOrDefault(i => 
                        i.GetTypeInfo().IsGenericType && 
                        (
                            (typeof(IRepository<>)).GetTypeInfo().IsAssignableFrom(i) 
                            || (typeof(IRepository<,>)).GetTypeInfo().IsAssignableFrom(i)
                        )
                    );
                interfaceImpls.Add(new InterfaceImplPair() { Interface = @interface, Impl = t });
            });
            return interfaceImpls;
        }
    }
}
