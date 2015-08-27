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
using System.Globalization;
using SqlGenerater.Parser.Parts;

namespace SqlGenerater.Utils
{
    public static class SqlPartExtendsion
    {
        public static bool IsOpComparison(this Expression expression)
        {
            switch (expression.Op)
            {
                case SqlOperator.Equal:
                case SqlOperator.NotEqual:
                case SqlOperator.GreaterThan:
                case SqlOperator.GreaterThanEqual:
                case SqlOperator.LessThan:
                case SqlOperator.LessThanEqual:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetString(this SqlOperator op)
        {
            switch (op)
            {
                case SqlOperator.Equal:
                    return " = ";
                case SqlOperator.NotEqual:
                    return " <> ";
                case SqlOperator.GreaterThan:
                    return " > ";
                case SqlOperator.GreaterThanEqual:
                    return " >= ";
                case SqlOperator.LessThan:
                    return " < ";
                case SqlOperator.LessThanEqual:
                    return " <= ";
                case SqlOperator.And:
                    return " AND ";
                case SqlOperator.Or:
                    return " OR ";
            }
            throw new SqlVisitException("SqlOperator({0}) not support.", op);
        }

        public static string GetConstantString(this object value)
        {
            var type = value.GetType();
            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Boolean:
                    return ((bool)value) ? "1" : "0";
                case TypeCode.DBNull:
                    return "null";
                case TypeCode.DateTime:
                    return ((DateTime)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Empty:
                    return "''";
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return Convert.ToString(value);
                case TypeCode.Char:
                case TypeCode.String:
                    return "'" + value + "'";
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return ((byte)value).ToString();
                case TypeCode.Decimal:
                    return ((decimal)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Double:
                    return ((double)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Single:
                    return ((float)value).ToString(CultureInfo.InvariantCulture);
            }
            throw new SqlVisitException("TypeCode({0}) not support.", code);
        }
    }
}
