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
using System.Reflection;
using SqlGenerater.Parser;
using SqlGenerater.Parser.Parts;
using SqlGenerater.Query.Parts;
using SqlGenerater.Utils;

namespace SqlGenerater.Query
{
    public abstract class AbstractSqlDriver : ISqlDriver
    {
        private long _aliasCounter = 1;

        public virtual Alias CreateAlias(string name)
        {
            return new Alias(name);
        }

        public virtual Alias CreateAlias(Type type)
        {
            return new Alias(string.Format("t{0}", _aliasCounter++));
        }

        public abstract Select CreateSelect(Type type);

        public abstract Select CreateSelect(Select @select, Type type);

        public abstract TableBase CreateTable(string name, Type type);

        public abstract TableBase CreateTable(Select @select, Type type);

        public abstract LeftJoin CreateLeftJoin(TableBase table, Type type);

        public abstract RightJoin CreateRightJoin(TableBase table, Type type);

        public virtual Where CreateWhere(Expression condition)
        {
            return new Where(condition);
        }

        public virtual OrderBy CreateOrderBy()
        {
            return new OrderBy();
        }

        public virtual In CreateIn()
        {
            return new In();
        }

        public virtual Expression CreateExpression(SqlPart left, SqlOperator op, SqlPart right)
        {
            return new Expression(left, op, right);
        }

        public virtual Constant CreateConstant(object value)
        {
            return new Constant(value);
        }

        public virtual Condition CreateCondition(SqlPart left, SqlOperator op, SqlPart right)
        {
            if (op != SqlOperator.Or && op != SqlOperator.And)
                Assert.NotSupport("Condition not support SqlOperator: {0}", op);
            return new Condition(left, op, right);
        }

        public virtual Column CreateColumn(MemberInfo member, TableBase table)
        {
            return new Column(member.Name, table);
        }

        public virtual Column CreateColumn(MemberInfo member, Alias alias, TableBase table)
        {
            return new Column(member.Name, alias, table);
        }
    }
}
