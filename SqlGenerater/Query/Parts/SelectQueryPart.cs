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
using System.Linq;
using SqlGenerater.Parser.Parts;

namespace SqlGenerater.Query.Parts
{
    public sealed class SelectQueryPart : Select, ITableBaseQueryPart
    {
        private Type _type;

        // empty refrence
        private Alias _alias;

        public SelectQueryPart(Alias alias, Type type)
            : this(null, alias, type)
        {
        }

        public SelectQueryPart(Select select, Alias alias, Type type)
            : base(select, alias)
        {
            _type = type;
        }

        public TableBase FindTableBaseByType(Type type)
        {
            return FindTableBaseByType(type, out _alias);
        }

        public TableBase FindTableBaseByType(Type type, out Alias alias)
        {
            alias = null;
            var table = Tables.Where(m => (m as ITableBaseQueryPart) != null)
                            .Cast<ITableBaseQueryPart>()
                            .FirstOrDefault(m => m.Type == type) as TableBase;
            if (table == null)
                return null;
            alias = table.Alias;
            return table;
        }

        public Type Type
        {
            get { return _type; }
        }

        internal void ChangeType(Type type)
        {
            if (type != null && _type != type)
                _type = type;
        }
    }
}
