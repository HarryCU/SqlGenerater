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

namespace SqlGenerater.Query.Expressions.Translate
{
    [Translate(ExpressionType.New, typeof(Column))]
    internal class ColumnNewTranslater : AbstractTranslater<NewExpression>
    {
        protected override IEnumerable<SqlPart> DoTranslate(SqlPart current, NewExpression expression)
        {
            var select = (SelectQueryPart)current;
            var index = 0;
            foreach (var member in expression.Members)
            {
                var argument = (MemberExpression)expression.Arguments[index];
                var table = select.FindTableBaseByType(argument.Expression.Type);
                yield return Driver.CreateColumn(argument.Member, Driver.CreateAlias(member.Name), table);
                index++;
            }
        }
    }

    [Translate(ExpressionType.MemberAccess, typeof(Column))]
    internal class ColumnMemberTranslater : AbstractTranslater<MemberExpression>
    {
        protected override IEnumerable<SqlPart> DoTranslate(SqlPart current, MemberExpression expression)
        {
            var select = (SelectQueryPart)current;
            var table = select.FindTableBaseByType(expression.Expression.Type);
            yield return Driver.CreateColumn(expression.Member, table);
        }
    }
}
