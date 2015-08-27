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
    public class Expression : SqlPart
    {
        public static readonly Expression Empty = new Expression(new Constant("1"), SqlOperator.Equal, new Constant("1"));

        private readonly SqlPart _left;
        private readonly SqlPart _right;
        private readonly SqlOperator _op;

        public Expression(SqlPart left, SqlOperator op, SqlPart right)
        {
            Assert.CheckArgument(left, "left");
            Assert.CheckArgument(right, "right");

            _op = op;
            _left = left;
            _right = right;
        }

        public override SqlPartType PartType
        {
            get { return SqlPartType.Experssion; }
        }

        public SqlOperator Op
        {
            get { return _op; }
        }

        public SqlPart Left
        {
            get { return _left; }
        }

        public SqlPart Right
        {
            get { return _right; }
        }

        public override void Accept(ISqlVisitor visitor)
        {
            visitor.Visit(Left);
            visitor.Write(Op.GetString());
            visitor.Visit(Right);
        }

        public override string ToString()
        {
            return Left + Op.GetString() + Right;
        }
    }
}
