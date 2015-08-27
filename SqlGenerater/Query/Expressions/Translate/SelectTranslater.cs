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
using System.Linq;
using System.Linq.Expressions;
using SqlGenerater.Parser;
using SqlGenerater.Parser.Parts;
using SqlGenerater.Query.Parts;
using SqlGenerater.Utils;
using Expression = System.Linq.Expressions.Expression;

namespace SqlGenerater.Query.Expressions.Translate
{
    [Translate(ExpressionType.Lambda, typeof(Select))]
    internal sealed class SelectTranslater : AbstractTranslater<LambdaExpression>
    {
        protected override IEnumerable<SqlPart> DoTranslate(SqlPart current, LambdaExpression expression)
        {
            var body = expression.Body;

            //if (current == null)
            //{
            //    var typePart = Translate<TypePart>(null, body.UnpackToParamterExpr());
            //    current = Driver.CreateSelect(typePart.Type);
            //}

            var currentSelect = (current ?? Driver.CreateSelect(body.Type)) as SelectQueryPart;
            if (currentSelect == null)
                yield break;

            currentSelect.ChangeType(body.Type);

            switch (body.NodeType)
            {
                case ExpressionType.Parameter:
                    {
                        var table = CreateTable(currentSelect, body, currentSelect.AddTable);
                        currentSelect.AddColumns(CreateColumns(table, body.Type));
                    }
                    break;
                case ExpressionType.MemberAccess:
                case ExpressionType.New:
                    {
                        CreateTable(currentSelect, body.UnpackToParamterExpr(), currentSelect.AddTable);
                        currentSelect.AddColumns(TranslateList<Column>(currentSelect, body));
                    }
                    break;
                default:
                    Assert.NotSupport("Select not support ExpressionType: ", body.NodeType);
                    break;
            }
            yield return currentSelect;
        }

        private TableBase CreateTable(SelectQueryPart current, Expression expression, Action<TableBase> notFoundAppendAction)
        {
            Assert.CheckArgument(expression, "expression");
            Assert.CheckArgument(notFoundAppendAction, "notFoundAppendAction");

            Alias alias;
            var table = current.FindTableBaseByType(expression.Type, out alias);
            if (table == null)
            {
                table = Driver.CreateTable(expression.Type.Name, expression.Type);
                notFoundAppendAction(table);
            }
            return table;
        }

        private IEnumerable<SqlPart> CreateColumns(TableBase table, Type type)
        {
            var properties = type.GetProperties();
            return properties.Select(property => Driver.CreateColumn(property, table));
        }
    }
}
