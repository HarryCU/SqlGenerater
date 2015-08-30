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
using SqlGenerater.Query.Utils;
using SqlGenerater.Utils;

namespace SqlGenerater.Query
{
    public abstract class AbstractSqlDriver : ISqlDriver
    {
        private readonly NameBuilder _anyNameBuilder = NameBuilder.Build("A");
        private readonly NameBuilder _tableNameBuilder = NameBuilder.Build("T");
        private readonly NameBuilder _selectNameBuilder = NameBuilder.Build("S");

        protected NameBuilder AnyNameBuilder
        {
            get { return _anyNameBuilder; }
        }

        protected NameBuilder TableNameBuilder
        {
            get { return _tableNameBuilder; }
        }

        protected NameBuilder SelectNameBuilder
        {
            get { return _selectNameBuilder; }
        }

        private Alias CreateAlias(string name)
        {
            return new Alias(name);
        }

        protected Alias CreateAlias(Type type, NameBuilder nameBuilder)
        {
            return new Alias(nameBuilder.Next());
        }

        public virtual Alias CreateAlias(Type type)
        {
            return CreateAlias(type, AnyNameBuilder);
        }

        public abstract Parameter CreateParameter(MemberInfo member, object value);

        public virtual Select CreateSelect(Type type)
        {
            return new SelectQueryPart(CreateAlias(type, SelectNameBuilder), type);
        }

        public virtual Select CreateSelect(Select @select, Type type)
        {
            return new SelectQueryPart(@select, CreateAlias(type, SelectNameBuilder), type);
        }

        public virtual TableWithColumnBase CreateTable(Type type)
        {
            return new TableQueryPart(type.Name, CreateAlias(type, TableNameBuilder), type);
        }

        public virtual InnerJoin CreateInnerJoin(TableBase refrence, TableBase table, Expression condition)
        {
            return new InnerJoin(refrence, table, condition);
        }

        public virtual LeftJoin CreateLeftJoin(TableBase refrence, TableBase table, Expression condition)
        {
            return new LeftJoin(refrence, table, condition);
        }

        public virtual RightJoin CreateRightJoin(TableBase refrence, TableBase table, Expression condition)
        {
            return new RightJoin(refrence, table, condition);
        }

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

        public virtual Column CreateColumn(MemberInfo member, string aliasName, TableBase table)
        {
            if (member.Name == aliasName)
                return CreateColumn(member, table);
            return new Column(member.Name, CreateAlias(aliasName), table);
        }

        public virtual Column CreateColumn(MemberInfo member, MemberInfo aliasMember, TableBase table)
        {
            return CreateColumn(member, aliasMember.Name, table);
        }
    }
}
