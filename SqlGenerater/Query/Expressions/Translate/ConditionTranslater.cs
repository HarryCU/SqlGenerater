using System;
using System.Linq.Expressions;
using SqlGenerater.Parser;
using SqlGenerater.Parser.Parts;
using SqlGenerater.Query.Parts;
using SqlGenerater.Query.Utils;
using SqlGenerater.Utils;
using Expression = System.Linq.Expressions.Expression;

namespace SqlGenerater.Query.Expressions.Translate
{
    internal abstract class ConditionTranslater<TExpression> : AbstractTranslater<TExpression>
         where TExpression : Expression
    {
        protected Parser.Parts.Expression CreateCondition(SelectQueryPart current, Expression expression, Func<Expression, object> queryDataHanlder = null)
        {
            var binary = expression.Cast<BinaryExpression>();
            // 最终目标
            if (binary.IsOpComparison())
                return CreateExpression(current, binary, queryDataHanlder);

            SqlPart left = binary.Left.IsOpComparison() ? CreateExpression(current, binary.Left, queryDataHanlder) : CreateCondition(current, binary.Left);
            var op = binary.NodeType == ExpressionType.AndAlso ? SqlOperator.And : SqlOperator.Or;
            SqlPart right = binary.Right.IsOpComparison() ? CreateExpression(current, binary.Right, queryDataHanlder) : CreateCondition(current, binary.Right);

            var condition = Driver.CreateCondition(left, op, right);

            condition.NeedLeftBrace = binary.NeedsParentheses(binary.Left);
            condition.NeedRightBrace = binary.NeedsParentheses(binary.Right);

            return condition;
        }

        private Parser.Parts.Expression CreateExpression(SelectQueryPart current, Expression expression, Func<Expression, object> queryDataHanlder)
        {
            var binary = expression.Cast<BinaryExpression>();
            var left = CreateExpressionItem(current, binary.Left, binary.Right, queryDataHanlder);
            var op = ConvertOperator(binary.NodeType);
            var right = CreateExpressionItem(current, binary.Right, binary.Left, queryDataHanlder);
            return Driver.CreateExpression(left, op, right);
        }

        private SqlPart CreateExpressionItem(SelectQueryPart current, Expression expression, Expression memberExpression, Func<Expression, object> queryDataHanlder)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    {
                        if (expression.IsColumnExpression())
                            return Translate<Column>(current, expression, queryDataHanlder);
                        return Translate<Parameter>(current, expression, expr => memberExpression.GetMemberInfo());
                    }
                case ExpressionType.Constant:
                    return Translate<Constant>(current, expression, queryDataHanlder);
                default:
                    Assert.NotSupport("Where not support ExpressionType: {0}", expression.NodeType);
                    break;
            }
            return null;
        }

        private SqlOperator ConvertOperator(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.Equal:
                    return SqlOperator.Equal;
                case ExpressionType.NotEqual:
                    return SqlOperator.NotEqual;
                case ExpressionType.GreaterThan:
                    return SqlOperator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return SqlOperator.GreaterThanEqual;
                case ExpressionType.LessThan:
                    return SqlOperator.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return SqlOperator.LessThanEqual;
            }

            Assert.NotSupport("SqlOperator not support ExpressionType: {0}", expressionType);
            return SqlOperator.Unknow;
        }
    }
}
