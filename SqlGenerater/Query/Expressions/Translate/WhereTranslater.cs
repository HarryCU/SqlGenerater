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
    [TranslateUsage(ExpressionType.Lambda, typeof(Where))]
    internal sealed class WhereTranslater : ConditionTranslater<LambdaExpression>
    {
        protected override IEnumerable<SqlPart> DoTranslate(SqlPart current, LambdaExpression expression)
        {
            var binary = expression.Body.Cast<BinaryExpression>();
            if (binary == null)
                yield break;

            var select = current as SelectQueryPart;
            var condition = CreateCondition(select, binary);
            yield return Driver.CreateWhere(condition);
        }
    }
}
