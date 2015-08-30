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

using SqlGenerater.Query;
using SqlGenerater.Test.Model;
using Xunit;

namespace SqlGenerater.Test
{
    public class JoinTest
    {
        [Fact]
        public void Left_join_test_1()
        {
            var query = new SqlQuery<UserModel>();

            var sql = query
                .Where(m => m.Name == "4" && (m.Id >= 1 || m.Id >= 1) || m.Name == "2" && m.Name == "3")
                .Select(p => new { I = p.Id, N = p.Name })
                .LeftJoin<UserModel>((m, l) => m.I == l.Id)
                .Select((m, l) => new { m.I, l.Name })
                .GetQueryString();
        }

        [Fact]
        public void Left_join_test_2()
        {
            var query = new SqlQuery<UserModel>();

            var sql = query
                .Where(m => m.Name == "4" && (m.Id >= 1 || m.Id >= 1) || m.Name == "2" && m.Name == "3")
                .Select(p => new { I = p.Id, N = p.Name })
                .LeftJoin<UserModel>((m, l) => m.I == l.Id)
                .LeftJoin<UserModel>((m, l) => m.I == l.Id)
                .GetQueryString();
        }

        [Fact]
        public void Left_join_test_3()
        {
            var query = new SqlQuery<UserModel>();

            var sql = query
                .Where(m => m.Name == "4" && (m.Id >= 1 || m.Id >= 1) || m.Name == "2" && m.Name == "3")
                .Select(p => new { I = p.Id, N = p.Name })
                .LeftJoin<UserModel>((m, l) => m.I == l.Id)
                .LeftJoin<UserModel>((m, l) => m.I == l.Id)
                .Select((m, l) => new { m.I, l.Name })
                .GetQueryString();
        }

        [Fact]
        public void Right_join_test_1()
        {
            var query = new SqlQuery<UserModel>();

            var sql = query
                .Where(m => m.Name == "4" && (m.Id >= 1 || m.Id >= 1) || m.Name == "2" && m.Name == "3")
                .Select(p => new { I = p.Id, N = p.Name })
                .RightJoin<UserModel>((m, r) => m.I == r.Id)
                .Select((m, l) => new { m.I, l.Name })
                .GetQueryString();
        }

        [Fact]
        public void Inner_join_test_1()
        {
            var query = new SqlQuery<UserModel>();

            var sql = query
                .Where(m => m.Name == "4" && (m.Id >= 1 || m.Id >= 1) || m.Name == "2" && m.Name == "3")
                .Select(p => new { I = p.Id, N = p.Name })
                .InnerJoin<UserModel>((m, r) => m.I == r.Id)
                .Select((m, l) => new { m.I, l.Name })
                .GetQueryString();
        }
    }
}
