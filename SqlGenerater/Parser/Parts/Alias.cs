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
    public class Alias : SqlPart
    {
        private readonly string _value;

        public Alias(string value)
        {
            Assert.CheckArgument(value, "value");
            _value = value;
        }

        public override SqlPartType PartType
        {
            get { return SqlPartType.Alias; }
        }

        public override bool Equals(SqlPart other)
        {
            var alias = other as Alias;
            if (alias == null)
                return false;
            return alias._value == _value;
        }

        public override bool Equals(object obj)
        {
            var alias = obj as Alias;
            if (alias == null)
                return false;
            return alias._value.Equals(_value);
        }

        public override void Accept(ISqlVisitor visitor)
        {
            visitor.WriteKeyword(SqlKeyword.Blank);
            visitor.Write(ToString());
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
