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
using SqlGenerater.Query.Parts;

namespace SqlGenerater.Query.Expressions.Translate
{
    [TranslateUsage(ExpressionType.Parameter, typeof(TypePart))]
    internal class TypePartParameterTranslater : AbstractTranslater<ParameterExpression>
    {
        protected override IEnumerable<SqlPart> DoTranslate(SqlPart current, ParameterExpression expression)
        {
            yield return new TypePart(expression.Type);
        }
    }

    //[Translate(ExpressionType.New, typeof(TypePart))]
    //internal class TypePartNewTranslater : AbstractTranslater<ParameterExpression>
    //{
    //    protected override IEnumerable<SqlPart> DoTranslate(SqlPart current, ParameterExpression expression)
    //    {
    //        yield return new TypePart(expression.Type);
    //    }
    //}
}
