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
using System.Linq;
using SqlGenerater.Parser.Visitor;

namespace SqlGenerater.Parser.Parts
{
    public class OrderBy : SqlPart
    {
        public class OrderItem
        {
            private readonly OrderByDirection _direction;
            private readonly Column _column;

            public OrderItem(OrderByDirection direction, Column column)
            {
                _direction = direction;
                _column = column;
            }

            public Column Column
            {
                get { return _column; }
            }

            public OrderByDirection Direction
            {
                get { return _direction; }
            }
        }

        private readonly List<OrderItem> _items;

        public OrderBy()
        {
            _items = new List<OrderItem>();
        }

        public override SqlPartType PartType
        {
            get { return SqlPartType.OrderBy; }
        }

        public IReadOnlyList<OrderItem> Items
        {
            get { return _items; }
        }

        public void AddAsc(Column column)
        {
            Add(OrderByDirection.Asc, column);
        }

        public void Add(OrderByDirection direction, Column column)
        {
            _items.Add(new OrderItem(direction, column));
        }

        public override void Accept(ISqlVisitor visitor)
        {
            visitor.WriteKeyword(SqlKeyword.OrderBy);
            visitor.VisitParts(Items, item =>
            {
                visitor.Visit(item.Column);
                visitor.WriteKeyword(item.Direction == OrderByDirection.Asc ? SqlKeyword.Asc : SqlKeyword.Desc);
            });
        }

        public override string ToString()
        {
            return "ORDER BY " + string.Join(",", Items.Select(m => m.Column + (m.Direction == OrderByDirection.Asc ? "asc" : "desc")));
        }
    }
}
