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

using SqlGenerater.Parser.Visitor;
using SqlGenerater.Utils;

namespace SqlGenerater.Parser.Parts
{
    public class Column : SqlPart
    {
        private readonly string _name;
        private readonly Alias _alias;
        private readonly TableBase _table;

        public Column(string name, TableBase table)
            : this(name, null, table)
        {

        }

        public Column(string name, Alias @alias, TableBase table)
        {
            Assert.CheckArgument(name, "name");
            Assert.CheckArgument(table, "table");
            _name = name;
            _table = table;
            _alias = alias;
        }

        public override SqlPartType PartType
        {
            get { return SqlPartType.Column; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Alias TableAlais
        {
            get { return _table.Alias; }
        }

        public Alias Alias
        {
            get { return _alias; }
        }

        public override void Accept(ISqlVisitor visitor)
        {
            if (TableAlais == null)
                visitor.Write(Name);
            else
            {
                visitor.Write("{0}", TableAlais);
                visitor.WriteKeyword(SqlKeyword.Dot);
                visitor.Write("{0}", Name);
            }
            visitor.Visit(Alias);
        }

        public override string ToString()
        {
            return TableAlais == null ? Name : TableAlais + "." + Name;
        }
    }
}
