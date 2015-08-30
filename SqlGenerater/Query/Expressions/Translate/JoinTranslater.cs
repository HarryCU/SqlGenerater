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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SqlGenerater.Parser;
using SqlGenerater.Parser.Parts;
using SqlGenerater.Query.Parts;
using SqlGenerater.Utils;
using Expression = SqlGenerater.Parser.Parts.Expression;

namespace SqlGenerater.Query.Expressions.Translate
{
    internal abstract class JoinTranslater : ConditionTranslater<LambdaExpression>
    {

        protected Join CreateJoin(SqlPart current, LambdaExpression expression, Func<Select, TableBase, Expression, Join> createHanlder)
        {
            var select = current as SelectQueryPart;
            var binary = expression.Body.Cast<BinaryExpression>();
            if (binary == null || select == null)
                return null;

            var right = CreateTable(expression.Parameters[1].Type);
            var left = select.FindTableBaseByType(expression.Parameters[0].Type) ?? select;

            var leftParameterName = expression.Parameters[0].GetParamterExprName();

            var condition = CreateCondition(select, binary, expr =>
            {
                var parameterName = expr.GetParamterExprName();
                if (parameterName == null)
                    return null;

                if (leftParameterName == parameterName)
                    return left;
                return right;
            });
            return createHanlder(select, right, condition);
        }
    }

    [TranslateUsage(ExpressionType.Lambda, typeof(InnerJoin))]
    internal sealed class InnerJoinTranslater : JoinTranslater
    {
        protected override IEnumerable<SqlPart> DoTranslate(SqlPart current, LambdaExpression expression)
        {
            var join = CreateJoin(current, expression, Driver.CreateInnerJoin);

            if (join == null)
                yield break;

            yield return join;
        }
    }

    [TranslateUsage(ExpressionType.Lambda, typeof(LeftJoin))]
    internal sealed class LeftJoinTranslater : JoinTranslater
    {
        protected override IEnumerable<SqlPart> DoTranslate(SqlPart current, LambdaExpression expression)
        {
            var join = CreateJoin(current, expression, Driver.CreateLeftJoin);

            if (join == null)
                yield break;

            yield return join;
        }
    }

    [TranslateUsage(ExpressionType.Lambda, typeof(RightJoin))]
    internal sealed class RightJoinTranslater : JoinTranslater
    {
        protected override IEnumerable<SqlPart> DoTranslate(SqlPart current, LambdaExpression expression)
        {
            var join = CreateJoin(current, expression, Driver.CreateRightJoin);

            if (join == null)
                yield break;

            yield return join;
        }
    }
}
