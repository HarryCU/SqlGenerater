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
using SqlGenerater.Parser.Parts;

namespace SqlGenerater.Parser.Visitor
{
    public abstract class RawSqlStringVisitor : AbstractSqlVisitor
    {
        private readonly Stack<Select> _stackSelects = new Stack<Select>(5);

        protected int SelectLevel
        {
            get { return _stackSelects.Peek().Level; }
        }

        protected override void VisitAlias(Alias alias)
        {
            Accept(alias);
        }

        protected override void VisitSelect(Select @select)
        {
            _stackSelects.Push(@select);
            Accept(@select);
            _stackSelects.Pop();
        }

        protected override void VisitWhere(Where @where)
        {
            Accept(@where);
        }

        protected override void VisitTable(TableBase table)
        {
            Accept(table);
        }

        protected override void VisitJoin(Join @join)
        {
            Accept(@join);
        }

        protected override void VisitOrderBy(OrderBy orderBy)
        {
            Accept(orderBy);
        }

        protected override void VisitIn(In @in)
        {
            Accept(@in);
        }

        protected override void VisitExpression(Expression expression)
        {
            Accept(expression);
        }

        protected override void VisitConstant(Constant constant)
        {
            Accept(constant);
        }

        protected override void VisitCondition(Condition condition)
        {
            Accept(condition);
        }

        protected override void VisitColumn(Column column)
        {
            Accept(column);
        }
    }
}
