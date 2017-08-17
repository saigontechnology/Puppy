#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> PropertyExtensions.cs </Name>
//         <Created> 16/08/17 6:08:02 PM </Created>
//         <Key> b207be99-bed7-45d7-8d0b-c26690d5444b </Key>
//     </File>
//     <Summary>
//         PropertyExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Puppy.Core.ObjectUtils
{
    public static class PropertyExtensions
    {
        private const string ExpressionCannotBeNullMessage = "The expression cannot be null.";
        private const string InvalidExpressionMessage = "Invalid expression.";

        public static string GetMemberName<T>(this T instance, Expression<Func<T, object>> expression)
        {
            return GetMemberName(expression.Body);
        }

        public static List<string> GetMemberNames<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            List<string> memberNames = new List<string>();
            foreach (var cExpression in expressions)
            {
                memberNames.Add(GetMemberName(cExpression.Body));
            }
            return memberNames;
        }

        public static string GetMemberName<T>(this T instance, Expression<Action<T>> expression)
        {
            return GetMemberName(expression.Body);
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(ExpressionCannotBeNullMessage);
            }
            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                return memberExpression.Member.Name;
            }
            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression = (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }
            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMemberName(unaryExpression);
            }
            throw new ArgumentException(InvalidExpressionMessage);
        }

        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression = (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.Name;
            }
            return ((MemberExpression)unaryExpression.Operand).Member.Name;
        }
    }
}