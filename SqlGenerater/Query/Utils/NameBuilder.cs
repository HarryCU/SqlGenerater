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

using SqlGenerater.Utils;

namespace SqlGenerater.Query.Utils
{
    public sealed class NameBuilder
    {
        private long _counter;
        private readonly string _prefix;

        private NameBuilder(string prefix)
        {
            Assert.CheckArgument(prefix, "prefix");
            _counter = 0;
            _prefix = prefix;
        }

        public string Next()
        {
            return string.Format("{0}{1}", _prefix, ++_counter);
        }

        public static NameBuilder Build(string prefix)
        {
            return new NameBuilder(prefix);
        }
    }
}
