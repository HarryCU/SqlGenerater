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
using SqlGenerater.Parser.Visitor;
using SqlGenerater.Utils;

namespace SqlGenerater.Parser.Parts
{
    public abstract class Join : TableBase
    {
        private readonly TableBase _refrence;
        private readonly TableBase _table;

        protected Join(TableBase refrence, TableBase table, Expression condition)
            : base(table.Alias)
        {
            _refrence = refrence;
            _table = table;
            Condition = condition;
        }

        public TableBase Refrence
        {
            get { return _refrence; }
        }

        public TableBase Table
        {
            get { return _table; }
        }

        public Expression Condition { get; private set; }

        public override IReadOnlyList<SqlPart> Columns
        {
            get { return _table.Columns; }
        }

        public override void Accept(ISqlVisitor visitor)
        {
            switch (PartType)
            {
                case SqlPartType.LeftJoin:
                    visitor.WriteKeyword(SqlKeyword.LeftJoin);
                    break;
                case SqlPartType.RightJoin:
                    visitor.WriteKeyword(SqlKeyword.RightJoin);
                    break;
                case SqlPartType.InnerJoin:
                    visitor.WriteKeyword(SqlKeyword.InnerJoin);
                    break;
            }
            visitor.Visit(Table);
            visitor.WriteKeyword(SqlKeyword.On);
            visitor.Visit(Condition);
        }
    }
}
