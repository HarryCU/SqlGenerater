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
using System.Linq.Expressions;
using SqlGenerater.Parser.Parts;
using SqlGenerater.Query.Expressions;
using SqlGenerater.Query.Parts;

namespace SqlGenerater.Query
{
    public class JoinSqlQuery<TModel, TJoinModel> : SqlQuery<TModel>, IJoinSqlQuery<TModel, TJoinModel>
    {
        private readonly Join _join;

        protected Join Join
        {
            get { return _join; }
        }

        public JoinSqlQuery(ISqlDriver driver, Join @join)
            : base(driver)
        {
            _join = @join;
        }

        internal JoinSqlQuery(SqlQueryBase query, Join @join)
            : base(query)
        {
            _join = @join;
        }

        protected Select CreateSelect<TResult>(Expression<Func<TModel, TJoinModel, TResult>> expression)
        {
            if (ParentSelectPart != null)
                SelectPart = Driver.CreateSelect(ParentSelectPart, typeof(TModel));
            var current = ((SelectQueryPart)SelectPart).FindTableBaseByType(expression.Parameters[0].Type);
            return Translater.Translate<Select>(Driver, SelectPart, expression, expr => current);
        }

        public ISqlQuery<TResult> Select<TResult>(Expression<Func<TModel, TJoinModel, TResult>> expression)
        {
            SelectPart = CreateSelect(expression);
            return new JoinSqlQuery<TResult, TJoinModel>(this, Join);
        }
    }
}
