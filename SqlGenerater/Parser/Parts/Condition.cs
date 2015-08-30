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
    public class Condition : Expression
    {
        public override SqlPartType PartType
        {
            get { return SqlPartType.Condition; }
        }

        internal bool NeedLeftBrace { get; set; }
        internal bool NeedRightBrace { get; set; }

        public Condition(SqlPart left, SqlOperator op, SqlPart right)
            : base(left, op, right)
        {
            NeedLeftBrace = NeedRightBrace = false;
        }

        private void WriteBrace(bool needBrace, ISqlVisitor visitor, SqlPart part)
        {
            if (needBrace)
                visitor.WriteKeyword(SqlKeyword.LeftBrace);
            visitor.Visit(part);
            if (needBrace)
                visitor.WriteKeyword(SqlKeyword.RightBrace);
        }

        public override void Accept(ISqlVisitor visitor)
        {
            WriteBrace(NeedLeftBrace, visitor, Left);
            visitor.Write(Op.GetString());
            WriteBrace(NeedRightBrace, visitor, Right);
        }
    }
}
