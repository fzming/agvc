using System;
using System.Linq.Expressions;

namespace Utility.Extensions
{
    public static class ExpressionExtensions
    {
        public static MemberExpression GetMemberExpression<T>(this Expression<Func<T, object>> exp) where T : class
        {
            //https://stackoverflow.com/questions/12975373/expression-for-type-members-results-in-different-expressions-memberexpression
            var member = exp.Body as MemberExpression;
            var unary = exp.Body as UnaryExpression;
            return member ?? unary?.Operand as MemberExpression;
        }
    }
}