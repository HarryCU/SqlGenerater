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
using SqlGenerater.Utils;

namespace SqlGenerater.Parser.Parts
{
    public abstract class TableWithColumnBase : TableBase
    {
        private readonly List<SqlPart> _columns;

        public override IReadOnlyList<SqlPart> Columns
        {
            get { return _columns; }
        }

        public void AddColumn(SqlPart column)
        {
            Assert.CheckNull(column);

            _columns.Add(column);
        }

        public void AddColumns(IEnumerable<SqlPart> columns)
        {
            _columns.AddRange(columns);
        }

        protected TableWithColumnBase(Alias alias)
            : base(alias)
        {
            _columns = new List<SqlPart>();
        }
    }
}
