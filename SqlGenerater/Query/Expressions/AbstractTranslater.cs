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
using System.Linq.Expressions;
using SqlGenerater.Parser;

namespace SqlGenerater.Query.Expressions
{
    internal abstract class AbstractTranslater<TExpression> : ITranslater
        where TExpression : Expression
    {
        public ISqlDriver Driver
        {
            get;
            private set;
        }

        public IEnumerable<SqlPart> Translate(ISqlDriver driver, SqlPart current, Expression expression)
        {
            Driver = driver;
            return DoTranslate(current, expression as TExpression);
        }

        protected abstract IEnumerable<SqlPart> DoTranslate(SqlPart current, TExpression expression);

        protected TPart Translate<TPart>(SqlPart current, Expression expression)
            where TPart : SqlPart
        {
            return Translater.Translate<TPart>(Driver, current, expression);
        }

        protected IEnumerable<TPart> TranslateList<TPart>(SqlPart current, Expression expression)
            where TPart : SqlPart
        {
            return Translater.TranslateList<TPart>(Driver, current, expression);
        }
    }
}
