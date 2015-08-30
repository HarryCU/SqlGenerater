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
using System.Collections.Generic;
using System.Linq;
using SqlGenerater.Parser;
using SqlGenerater.Parser.Parts;
using Expression = System.Linq.Expressions.Expression;

namespace SqlGenerater.Query.Expressions
{
    internal abstract class AbstractTranslater<TExpression> : ITranslater
        where TExpression : Expression
    {
        public ISqlDriver Driver { get; private set; }

        protected Func<Expression, object> QueryDataHanlder { get; private set; }

        public IEnumerable<SqlPart> Translate(ISqlDriver driver, SqlPart current, Expression expression,
            Func<Expression, object> queryDataHanlder)
        {
            Driver = driver;
            QueryDataHanlder = queryDataHanlder;
            return DoTranslate(current, expression as TExpression);
        }

        protected abstract IEnumerable<SqlPart> DoTranslate(SqlPart current, TExpression expression);


        protected TData GetData<TData>(Expression expression)
        {
            if (QueryDataHanlder != null)
                return (TData)QueryDataHanlder(expression);
            return default(TData);
        }

        protected IEnumerable<SqlPart> CreateColumns(TableBase table, Type type)
        {
            var properties = type.GetProperties();
            return properties.Select(property => Driver.CreateColumn(property, table));
        }

        protected TableWithColumnBase CreateTable(Type type)
        {
            var table = Driver.CreateTable(type);
            table.AddColumns(CreateColumns(table, type));
            return table;
        }


        // NOTE: Translater.Translate 便捷使用
        protected TPart Translate<TPart>(SqlPart current, Expression expression, Func<Expression, object> queryDataHanlder = null)
            where TPart : SqlPart
        {
            return Translater.Translate<TPart>(Driver, current, expression, queryDataHanlder ?? QueryDataHanlder);
        }

        protected IEnumerable<TPart> TranslateList<TPart>(SqlPart current, Expression expression,
            Func<Expression, object> queryDataHanlder = null)
            where TPart : SqlPart
        {
            return Translater.TranslateList<TPart>(Driver, current, expression, queryDataHanlder ?? QueryDataHanlder);
        }
    }
}
