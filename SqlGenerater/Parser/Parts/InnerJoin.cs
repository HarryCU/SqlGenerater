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

namespace SqlGenerater.Parser.Parts
{
    public class InnerJoin : Join
    {
        public override SqlPartType PartType
        {
            get { return SqlPartType.InnerJoin; }
        }

        public InnerJoin(TableBase refrence, TableBase table, Expression condition)
            : base(refrence, table, condition)
        {
        }
    }
}
