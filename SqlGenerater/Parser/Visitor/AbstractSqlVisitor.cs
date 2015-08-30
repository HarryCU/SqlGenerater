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
using System.Reflection;
using SqlGenerater.Parser.Parts;
using SqlGenerater.Utils;

namespace SqlGenerater.Parser.Visitor
{
    public abstract class AbstractSqlVisitor : ISqlVisitor
    {
        public virtual void Visit(SqlPart part)
        {
            if (part == null)
                return;
            switch (part.PartType)
            {
                case SqlPartType.Column:
                    VisitColumn(part as Column);
                    break;
                case SqlPartType.Condition:
                    VisitCondition(part as Condition);
                    break;
                case SqlPartType.Constant:
                    VisitConstant(part as Constant);
                    break;
                case SqlPartType.Experssion:
                    VisitExpression(part as Expression);
                    break;
                case SqlPartType.In:
                    VisitIn(part as In);
                    break;
                case SqlPartType.OrderBy:
                    VisitOrderBy(part as OrderBy);
                    break;
                case SqlPartType.RightJoin:
                case SqlPartType.LeftJoin:
                case SqlPartType.InnerJoin:
                    VisitJoin(part as Join);
                    break;
                case SqlPartType.Select:
                    VisitSelect(part as Select);
                    break;
                case SqlPartType.Table:
                    VisitTable(part as Table);
                    break;
                case SqlPartType.Where:
                    VisitWhere(part as Where);
                    break;
                case SqlPartType.Alias:
                    VisitAlias(part as Alias);
                    break;
                case SqlPartType.Parameter:
                    VisitParameter(part as Parameter);
                    break;
            }
        }

        public abstract void Write(string text, params object[] args);

        public virtual void WriteKeyword(SqlKeyword keyword)
        {
            string word = null;
            switch (keyword)
            {
                case SqlKeyword.Select:
                    word = "SELECT ";
                    break;
                case SqlKeyword.From:
                    word = " FROM ";
                    break;
                case SqlKeyword.Where:
                    word = " WHERE ";
                    break;
                case SqlKeyword.LeftBrace:
                    word = "(";
                    break;
                case SqlKeyword.RightBrace:
                    word = ")";
                    break;
                case SqlKeyword.LeftJoin:
                    word = "LEFT JOIN ";
                    break;
                case SqlKeyword.RightJoin:
                    word = "RIGHT JOIN ";
                    break;
                case SqlKeyword.InnerJoin:
                    word = "INNER JOIN ";
                    break;
                case SqlKeyword.On:
                    word = " ON ";
                    break;
                case SqlKeyword.OrderBy:
                    word = " ORDER BY ";
                    break;
                case SqlKeyword.Asc:
                    word = " ASC";
                    break;
                case SqlKeyword.Desc:
                    word = " DESC";
                    break;
                case SqlKeyword.Dot:
                    word = ".";
                    break;
                case SqlKeyword.Comma:
                    word = ", ";
                    break;
                case SqlKeyword.Blank:
                    word = " ";
                    break;
            }
            if (word != null)
                Write(word);
        }

        public abstract void WriteParameter(Parameter parameter);

        protected virtual void Accept(SqlPart part)
        {
            part.Accept(this);
        }

        protected abstract void VisitAlias(Alias alias);
        protected abstract void VisitSelect(Select select);
        protected abstract void VisitParameter(Parameter parameter);
        protected abstract void VisitWhere(Where where);
        protected abstract void VisitTable(TableBase table);
        protected abstract void VisitJoin(Join join);
        protected abstract void VisitOrderBy(OrderBy orderBy);
        protected abstract void VisitIn(In @in);
        protected abstract void VisitExpression(Expression expression);
        protected abstract void VisitConstant(Constant constant);
        protected abstract void VisitCondition(Condition condition);
        protected abstract void VisitColumn(Column column);

        public void VisitParts<T>(IEnumerable<T> datas)
            where T : SqlPart
        {
            VisitParts(datas, Visit);
        }

        public void VisitParts<T>(IEnumerable<T> datas, Action<T> action)
        {
            Assert.CheckArgument(action, "action");
            bool first = true;
            foreach (var data in datas)
            {
                if (!first)
                    WriteKeyword(SqlKeyword.Comma);
                action(data);

                first = false;
            }
        }
    }
}
