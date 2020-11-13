using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Resolving.Pipeline;
using Infrastructure.Autofac.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Module = Autofac.Module;

namespace Infrastructure.Autofac.Modules
{
    public class PropertyAutowiredModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException("registration");
            }

            registration.PipelineBuilding += RegistrationPipelineBuilding;
        }

        private void RegistrationPipelineBuilding(object sender, IResolvePipelineBuilder builder)
        {
            builder.Use(new PropertyAutoWriteMiddleware());
        }
    }

    public class PropertyAutoWriteMiddleware : IResolveMiddleware
    {
        public PipelinePhase Phase => PipelinePhase.Activation;

        public void Execute(ResolveRequestContext context, Action<ResolveRequestContext> next)
        {
            next(context);
            var instance = context.Instance;
            if (instance != null)
            {
                var actions = GetResolveProperties(instance.GetType());
                if (actions != null)
                {
                    foreach (var a in actions)
                    {
                        a(context, instance);
                    }
                }
            }
        }

        private static readonly Dictionary<Type, Action<IComponentContext, object>[]> ResolvePropertiesActions = new Dictionary<Type, Action<IComponentContext, object>[]>();

        public static Action<IComponentContext, object>[] GetResolveProperties(Type type)
        {
            Action<IComponentContext, object>[] actions = null;

            if (!ResolvePropertiesActions.TryGetValue(type, out actions))
            {
                lock (ResolvePropertiesActions)
                    if (!ResolvePropertiesActions.TryGetValue(type, out actions))
                    {
                        var actionList = new List<Action<IComponentContext, object>>();
                        //Autowire Properties
                        foreach (PropertyInfo p in type.GetProperties())
                        {
                            var resolve = p.GetCustomAttributes().FirstOrDefault(t => t is IAutofacResolveAttribute) as IAutofacResolveAttribute;

                            if (p.CanWrite && resolve != null)
                            {
                                actionList.Add((IComponentContext context, object obj) =>
                                {
                                    resolve.Resolve(obj, p, context);
                                });
                            }
                            else if (resolve != null)
                            {
                                throw new NotSupportedException("IAutofacResolveAttribute can not apply to readonly property");
                            }
                        }
                        //Autowire fields
                        foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                        {
                            var resolve = f.GetCustomAttributes().FirstOrDefault(t => t is IAutofacResolveAttribute) as IAutofacResolveAttribute;
                            if (resolve != null)
                            {
                                actionList.Add((IComponentContext context, object obj) =>
                                {
                                    resolve.Resolve(obj, f, context);
                                });
                            }
                        }
                        foreach (var f in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                        {
                            var resolve = f.GetCustomAttributes().FirstOrDefault(t => t is IAutofacResolveAttribute) as IAutofacResolveAttribute;
                            if (resolve != null)
                            {
                                actionList.Add((IComponentContext context, object obj) =>
                                {
                                    resolve.Resolve(obj, f, context);
                                });
                            }
                        }

                        actions = actionList.ToArray();
                        ResolvePropertiesActions.Add(type, actions);
                    }
            }
            return actions;

        }
    }

}
