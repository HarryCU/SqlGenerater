using System.Linq.Expressions;
using SqlGenerater.Utils;

namespace SqlGenerater.Query.Utils
{
    public static class ExpressionExtendsion
    {
        public static bool IsColumnExpression(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var member = expression.Cast<MemberExpression>();
                return member.Expression.NodeType == ExpressionType.Parameter;
            }
            return false;
        }
    }
}
