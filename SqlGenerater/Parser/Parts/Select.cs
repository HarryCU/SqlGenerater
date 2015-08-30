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
using SqlGenerater.Parser.Visitor;
using SqlGenerater.Utils;

namespace SqlGenerater.Parser.Parts
{
    public abstract class Select : TableWithColumnBase
    {
        private readonly List<TableBase> _tables;

        protected Select()
            : this(null)
        {
        }

        protected Select(Alias alias)
            : base(alias)
        {
            _tables = new List<TableBase>();
            Parent = null;
        }

        protected Select(Select select, Alias alias)
            : this(alias)
        {
            if (select != null)
            {
                select.Parent = this;
                AddTable(select);
            }
        }

        public int Level
        {
            get { return CompluteLevel(); }
        }

        public Select Parent { get; private set; }

        protected bool HasParent
        {
            get { return Parent != null; }
        }

        public override SqlPartType PartType
        {
            get { return SqlPartType.Select; }
        }

        public IReadOnlyList<TableBase> Tables
        {
            get { return _tables; }
        }

        public bool HasTable
        {
            get { return Tables.Count > 0; }
        }

        public Where Where { get; internal set; }

        public OrderBy OrderBy { get; set; }

        public void AddTable(TableBase table)
        {
            Assert.CheckArgument(table, "table");
            AddTableBase(table);
        }

        public void AddTable(Join join)
        {
            Assert.CheckArgument(join, "table");

            AddTableBase(join, () =>
            {
                AddColumns(join.Columns);
            });
        }

        private void AddTableBase(TableBase table, Action action = null)
        {
            if (!_tables.Contains(table))
            {
                _tables.Add(table);
                if (action != null)
                    action();
            }
        }

        public TableBase FindTableBase(Alias alias)
        {
            return Tables.FirstOrDefault(m => alias.Equals(m.Alias));
        }

        private int CompluteLevel()
        {
            var level = 0;
            var current = this;
            while (current.Parent != null)
            {
                level++;
                current = current.Parent;
            }
            return level;
        }

        public override void Accept(ISqlVisitor visitor)
        {
            if (HasParent)
                visitor.WriteKeyword(SqlKeyword.LeftBrace);


            visitor.WriteKeyword(SqlKeyword.Select);

            visitor.VisitParts(Columns);

            if (HasTable)
            {
                visitor.WriteKeyword(SqlKeyword.From);
                foreach (var table in Tables)
                {
                    visitor.Visit(table);
                }
            }

            if (Where != null)
                visitor.Visit(Where);

            if (OrderBy != null)
                visitor.Visit(OrderBy);

            if (HasParent)
            {
                visitor.WriteKeyword(SqlKeyword.RightBrace);
                visitor.Visit(Alias);
            }

            visitor.WriteKeyword(SqlKeyword.Blank);
        }
    }
}
