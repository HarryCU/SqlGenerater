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
using System.Text;
using SqlGenerater.Parser.Parts;
using SqlGenerater.Parser.Visitor;
using SqlGenerater.Query.Parts;

namespace SqlGenerater.Query
{
    internal sealed class SqlQueryVisitor : RawSqlStringVisitor
    {
        private bool _outputTabString = true;
        private readonly SqlString _builder;

        public SqlQueryVisitor()
        {
            _builder = new SqlString();
        }

        public SqlString SqlString
        {
            get { return _builder; }
        }

        public override void Write(string text, params object[] args)
        {
            _builder.Append(text, args);
        }

        private string CreateTabString(int fix = 0)
        {
            var builder = new StringBuilder(4 * SelectLevel);
            for (int i = SelectLevel - fix; i > 0; i--)
            {
                builder.Append("    ");
            }
            return builder.ToString();
        }

        protected override void VisitWhere(Where @where)
        {
            _outputTabString = false;
            base.VisitWhere(@where);
            _outputTabString = true;
        }

        protected override void VisitJoin(Join @join)
        {
            if (_outputTabString)
                Write("{0}{1}", Environment.NewLine, CreateTabString());
            base.VisitJoin(@join);
        }

        public override void WriteKeyword(SqlKeyword keyword)
        {
            if (_outputTabString && keyword == SqlKeyword.LeftBrace)
            {
                base.WriteKeyword(keyword);
                Write("{0}{1}", Environment.NewLine, CreateTabString());
            }
            else if (_outputTabString && keyword == SqlKeyword.RightBrace)
            {
                Write("{0}{1}", Environment.NewLine, CreateTabString(1));
                base.WriteKeyword(keyword);
            }
            else
            {
                base.WriteKeyword(keyword);
            }
        }

        public override void WriteParameter(Parameter parameter)
        {
            var p = parameter as ParameterQueryPart;
            if (p != null) _builder.AddParameter(p.Member, p.Key, p.Value);
        }

        public override string ToString()
        {
            return SqlString.ToString();
        }
    }
}
