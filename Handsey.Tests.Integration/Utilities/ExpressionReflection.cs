using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Utilities
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>https://handcraftsman.wordpress.com/2008/11/11/how-to-get-c-property-names-without-magic-strings/</remarks>
    public static class ExpressionReflection
    {
        public static string PropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        public static string PropertyName<T>(Expression<Action<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }
    }
}