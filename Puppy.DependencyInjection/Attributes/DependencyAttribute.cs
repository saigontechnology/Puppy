#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> Puppy.DependencyInjection.Attributes </Project>
//     <File>
//         <Name> DependencyAttribute </Name>
//         <Created> 30 Mar 17 8:56:29 PM </Created>
//         <Key> 78e5f7ac-0613-4173-963d-0677a301d2cc </Key>
//     </File>
//     <Summary>
//         DependencyAttribute
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Puppy.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class DependencyAttribute : Attribute
    {
        protected DependencyAttribute(ServiceLifetime dependencyType)
        {
            DependencyType = dependencyType;
        }

        public ServiceLifetime DependencyType { get; }

        public Type ServiceType { get; set; }

        public ServiceDescriptor BuildServiceDescriptor(TypeInfo type)
        {
            var serviceType = ServiceType ?? type.AsType();

            return new ServiceDescriptor(serviceType, type.AsType(), DependencyType);
        }
    }
}