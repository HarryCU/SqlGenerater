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

using System.Diagnostics;
using SqlGenerater.Parser.Parts;
using SqlGenerater.Parser.Visitor;
using SqlGenerater.Query;
using SqlGenerater.Query.Parts;
using SqlGenerater.Test.Model;
using Xunit;

namespace SqlGenerater.Test
{
    public class SelectTest
    {
        [Fact]
        public void Select_Test_1()
        {
            var select = new SelectQueryPart(new Alias("t1"), typeof(UserModel));

            var user = new TableQueryPart("USER", new Alias("t2"), typeof(UserModel));
            select.AddTable(user);

            var id = new Column("Id", user);
            select.AddColumn(id);
            select.AddColumn(new Column("Name", user));

            //select.Where = new Where(new Expression(id, SqlOperator.Equal, new Constant("1")));

            var visitor = new DebugViewSqlVisitor();
            select.Visit(visitor);
            Debug.WriteLine("");
        }

        [Fact]
        public void Query_Select_Test_1()
        {
            var query = new SqlQuery<UserModel>();

            var sql = query
                //.Where(m => m.Id == "1")
                .Select(m => m)
                .Select(p => new { I = p.Id, N = p.Name })
                .Select(m => m.I)
                .GetQueryString();
        }
    }
}
