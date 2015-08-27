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

namespace SqlGenerater.Query
{
    public interface ISqlDriver
    {
        Alias CreateAlias(string name);
        Alias CreateAlias(Type type);
        Select CreateSelect(Type type);
        Select CreateSelect(Select select, Type type);
        Where CreateWhere(Expression condition);
        TableBase CreateTable(string name, Type type);
        TableBase CreateTable(Select @select, Type type);
        LeftJoin CreateLeftJoin(TableBase table, Type type);
        RightJoin CreateRightJoin(TableBase table, Type type);
        OrderBy CreateOrderBy();
        In CreateIn();
        Expression CreateExpression(SqlPart left, SqlOperator op, SqlPart right);
        Condition CreateCondition(SqlPart left, SqlOperator op, SqlPart right);
        Constant CreateConstant(object value);
        Column CreateColumn(MemberInfo member, TableBase table);
        Column CreateColumn(MemberInfo member, Alias alias, TableBase table);
    }
}
