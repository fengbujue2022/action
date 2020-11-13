using Autofac;
using System.Reflection;

namespace Infrastructure.Autofac.Attributes
{
    public interface IAutofacResolveAttribute
    {
        void Resolve(object sender, PropertyInfo property, IComponentContext resolveContext);
        void Resolve(object sender, FieldInfo field, IComponentContext resolveContext);
    }
}
