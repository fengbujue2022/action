using Autofac;
using Infrastructure.Autofac.Attributes;
using System;
using System.Linq;
using System.Reflection;
using Module = Autofac.Module;

namespace Infrastructure.Autofac.Modules
{
    public class AutoResolveClassModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            #region Register All Assemby
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var assemblyName = assembly.GetName().Name;
                if (!assemblyName.StartsWith("System", StringComparison.OrdinalIgnoreCase)
                    && !assemblyName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)
                    && !assemblyName.StartsWith("Google", StringComparison.OrdinalIgnoreCase)
                )
                {
                    var autoResolveTypes = assembly.GetTypes().Where(
                        t => t.IsDefined(typeof(AutofacResolveAttribute))
                    );
                    foreach (var t in autoResolveTypes)
                    {
                        var attrs = t.GetCustomAttributes<AutofacResolveAttribute>();
                        foreach (var attr in attrs)
                        {
                            var r = builder.RegisterType(t);
                            if (attr.AsImplementedInterfaces)
                            {
                                r = r.AsImplementedInterfaces();
                            }
                            if (attr.AsSelf)
                            {
                                r = r.AsSelf();
                            }
                            if (attr.AsType && attr.Type != null)
                            {
                                r = r.As(attr.Type);
                            }
                            if (attr.SingleInstance)
                            {
                                r = r.SingleInstance();
                            }
                            if (attr.Keyed != null)
                                r.Keyed(attr.Keyed, attr.Type ?? t);
                            if (!string.IsNullOrWhiteSpace(attr.Named))
                                r.Named(attr.Named, attr.Type ?? t);

                        }
                    }
                }
            }
            #endregion

            base.Load(builder);
        }
    }
}
