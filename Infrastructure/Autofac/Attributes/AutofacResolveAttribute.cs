using Autofac;
using Microsoft.Extensions.Logging;
using System;

namespace Infrastructure.Autofac.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public class AutofacResolveAttribute : Attribute, IAutofacResolveAttribute
    {
        /// <summary>
        /// Default False, only available when apply to class 
        /// </summary>
        public bool SingleInstance { get; set; }
        /// <summary>
        /// Default True, only available when apply to class 
        /// </summary>
        public bool AsSelf { get; set; }

        /// <summary>
        /// Default False, only available when apply to class 
        /// </summary>
        public bool AsImplementedInterfaces { get; set; }

        /// <summary>
        /// Default False, only available when apply to class
        /// </summary>
        public bool AsType { get; set; }

        public string Named { get; set; }
        public object Keyed { get; set; }

        public Type Type { get; set; }
        public AutofacResolveAttribute()
        {
            this.AsSelf = true;
        }
        public virtual void Resolve(object sender, System.Reflection.PropertyInfo property, IComponentContext context)
        {

            var pType = this.Type ?? property.PropertyType;
            if (pType.IsAssignableTo<ILogger>())
            {
                var log = context.Resolve<ILoggerFactory>().CreateLogger(this.GetType().Name);
                property.SetValue(sender, log);
            }
            else
            {
                var named = this.Named;
                var keyed = this.Keyed;
                if (!string.IsNullOrWhiteSpace(named))
                {
                    var pInstance = context.ResolveNamed(named, pType);
                    property.SetValue(sender, pInstance);
                }
                else if (keyed != null)
                {
                    var pInstance = context.ResolveKeyed(keyed, pType);
                    property.SetValue(sender, pInstance);
                }
                else
                {
                    var pInstance = context.Resolve(pType);
                    property.SetValue(sender, pInstance);
                }
            }
        }

        public virtual void Resolve(object sender, System.Reflection.FieldInfo field, IComponentContext context)
        {
            var fType = this.Type ?? field.FieldType;


            if (fType.IsAssignableTo<ILogger>())
            {
                var log = context.Resolve<ILoggerFactory>().CreateLogger(this.GetType().Name);
                field.SetValue(sender, log);
            }
            else
            {

                var named = this.Named;
                var keyed = this.Keyed;
                if (!string.IsNullOrWhiteSpace(named))
                {
                    var pInstance = context.ResolveNamed(named, fType);
                    field.SetValue(sender, pInstance);
                }
                else if (keyed != null)
                {
                    var pInstance = context.ResolveKeyed(keyed, fType);
                    field.SetValue(sender, pInstance);
                }
                else
                {
                    var pInstance = context.Resolve(fType);
                    field.SetValue(sender, pInstance);
                }
            }
        }
    }
}
