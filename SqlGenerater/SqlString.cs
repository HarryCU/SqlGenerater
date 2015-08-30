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
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SqlGenerater
{
    [DebuggerDisplay("{DebugView}")]
    public class SqlString
    {
        private readonly StringBuilder _builder;
        private readonly List<SqlParameter> _parameters;

        internal string DebugView
        {
            get { return ToString(); }
        }

        public SqlString()
        {
            _builder = new StringBuilder();
            _parameters = new List<SqlParameter>();
        }

        public IReadOnlyCollection<SqlParameter> Parameters
        {
            get { return _parameters.AsReadOnly(); }
        }

        internal void Append(string text, params object[] args)
        {
            _builder.AppendFormat(text, args);
        }

        internal void AddParameter(MemberInfo member, string key, object value)
        {
            _parameters.Add(new SqlParameter(key, value, member));
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
