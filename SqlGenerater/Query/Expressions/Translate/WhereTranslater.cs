/**
 * Copyright (c) 2015, Harry CU 邱允根 (292350862@qq.com).
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq.Expressions;
using SqlGenerater.Parser;
using SqlGenerater.Parser.Parts;
using SqlGenerater.Query.Parts;
using SqlGenerater.Utils;
using Expression = System.Linq.Expressions.Expression;
using ExpressionPart = SqlGenerater.Parser.Parts.Expression;

namespace SqlGenerater.Query.Expressions.Translate
{
    [Translate(ExpressionType.Lambda, typeof(Where))]
    internal sealed class WhereTranslater : AbstractTranslater<LambdaExpression>
    {
        protected override IEnumerable<SqlPart> DoTranslate(SqlPart current, LambdaExpression expression)
        {
            var binary = expression.Body as BinaryExpression;
            if (binary == null)
                yield break;

            var select = current as SelectQueryPart;
            var condition = CreateCondition(select, binary);
            yield return Driver.CreateWhere(condition);
        }

        private ExpressionPart CreateCondition(SelectQueryPart current, Expression expression)
        {
            var binary = (BinaryExpression)expression;
            // 最终目标
            if (binary.IsOpComparison())
                return CreateExpression(current, binary);

            SqlPart left = binary.Left.IsOpComparison() ? CreateExpression(current, binary.Left) : CreateCondition(current, binary.Left);
            var op = binary.NodeType == ExpressionType.AndAlso ? SqlOperator.And : SqlOperator.Or;
            SqlPart right = binary.Right.IsOpComparison() ? CreateExpression(current, binary.Right) : CreateCondition(current, binary.Right);

            var condition = Driver.CreateCondition(left, op, right);

            condition.NeedLeftBrace = binary.NeedsParentheses(binary.Left);
            condition.NeedRightBrace = binary.NeedsParentheses(binary.Right);

            return condition;
        }

        private ExpressionPart CreateExpression(SelectQueryPart current, Expression expression)
        {
            var binary = (BinaryExpression)expression;
            var left = CreateExpressionItem(current, binary.Left);
            var op = ConvertOperator(binary.NodeType);
            var right = CreateExpressionItem(current, binary.Right);
            return Driver.CreateExpression(left, op, right);
        }

        private SqlPart CreateExpressionItem(SelectQueryPart current, Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return Translate<Column>(current, expression);
                case ExpressionType.Constant:
                    return Translate<Constant>(current, expression);
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
