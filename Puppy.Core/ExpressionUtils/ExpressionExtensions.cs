#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ExpressionExtensions.cs </Name>
//         <Created> 27/11/2017 12:34:40 AM </Created>
//         <Key> 79182cb6-cf84-480f-adf8-6f3cce357a32 </Key>
//     </File>
//     <Summary>
//         ExpressionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Puppy.Core.ExpressionUtils
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyAccess(this LambdaExpression propertyAccessExpression)
        {
            var parameterExpression = propertyAccessExpression.Parameters.Single();
            var body = propertyAccessExpression.Body;
            var propertyInfo = parameterExpression.MatchSimplePropertyAccess(body);

            if (propertyInfo == null)
            {
                throw new ArgumentException(CoreStrings.InvalidPropertyExpression(propertyAccessExpression), nameof(propertyAccessExpression));
            }

            var declaringType = propertyInfo.DeclaringType;

            var type = parameterExpression.Type;

            if (declaringType == null || declaringType == type || !declaringType.GetTypeInfo().IsInterface ||
                !declaringType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                return propertyInfo;
            }

            var propertyGetter = propertyInfo.GetMethod;

            var runtimeInterfaceMap = type.GetTypeInfo().GetRuntimeInterfaceMap(declaringType);

            var index = Array.FindIndex(runtimeInterfaceMap.InterfaceMethods, p => propertyGetter.Equals(p));

            var targetMethod = runtimeInterfaceMap.TargetMethods[index];

            foreach (var runtimeProperty in type.GetRuntimeProperties())
            {
                if (targetMethod.Equals(runtimeProperty.GetMethod))
                {
                    return runtimeProperty;
                }
            }
            return propertyInfo;
        }

        private static PropertyInfo MatchSimplePropertyAccess(this Expression parameterExpression, Expression propertyAccessExpression)
        {
            IReadOnlyList<PropertyInfo> propertyInfoList = parameterExpression.MatchPropertyAccess(propertyAccessExpression);
            if (propertyInfoList == null || propertyInfoList.Count != 1)
                return null;
            return propertyInfoList.FirstOrDefault();
        }

        private static IReadOnlyList<PropertyInfo> MatchPropertyAccess(this Expression parameterExpression, Expression propertyAccessExpression)
        {
            var propertyInfoList = new List<PropertyInfo>();

            MemberExpression memberExpression;
            do
            {
                memberExpression = propertyAccessExpression.RemoveConvert() as MemberExpression;

                var propertyInfo = memberExpression?.Member as PropertyInfo;

                if (propertyInfo == null)
                {
                    return null;
                }

                propertyInfoList.Insert(0, propertyInfo);

                propertyAccessExpression = memberExpression.Expression;
            }
            while (memberExpression.Expression.RemoveConvert() != parameterExpression);
            return propertyInfoList;
        }
    }
}