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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SqlGenerater.Parser;
using SqlGenerater.Reflection;

namespace SqlGenerater.Query.Expressions
{
    internal sealed class Translater
    {
        private static readonly IDictionary<ExpressionType, IDictionary<Type, ITranslater>> Translaters = new ConcurrentDictionary<ExpressionType, IDictionary<Type, ITranslater>>();

        static Translater()
        {
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var attrData in Enumerable.Select(assembly.GetTypes()
                    .Where(ReflectionHelper.HasAttribute<TranslateAttribute>), m => new { Type = m, AttributeData = ReflectionHelper.GetAttributeOne<TranslateAttribute>(m) }).ToList())
            {
                if (!Translaters.ContainsKey(attrData.AttributeData.ExpressionType))
                    Translaters.Add(attrData.AttributeData.ExpressionType, new ConcurrentDictionary<Type, ITranslater>());

                Translaters[attrData.AttributeData.ExpressionType].Add(attrData.AttributeData.SqlPartType, attrData.Type.NewDef() as ITranslater);
            }
        }

        public static TPart Translate<TPart>(ISqlDriver driver, SqlPart current, Expression expression)
            where TPart : SqlPart
        {
            return TranslateList<TPart>(driver, current, expression).FirstOrDefault();
        }

        public static IEnumerable<TPart> TranslateList<TPart>(ISqlDriver driver, SqlPart current, Expression expression)
            where TPart : SqlPart
        {
            var type = typeof(TPart);
            if (Translaters.ContainsKey(expression.NodeType) && Translaters[expression.NodeType].ContainsKey(type))
            {
                return Translaters[expression.NodeType][type].Translate(driver, current, expression).Cast<TPart>();
            }
            return null;
        }
    }
}
