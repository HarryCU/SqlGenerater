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
using System.Reflection;

namespace SqlGenerater.Reflection.Implements
{
    internal class PropertyImplement : MemberImplement, IProperty
    {
        private Func<object, object> _getter;
        private MethodImplement _setter;

        public bool IsStatic
        {
            get { return Property.GetGetMethod(true).IsStatic; }
        }

        public PropertyInfo Property
        {
            get { return Member as PropertyInfo; }
        }

        public Type Type { get { return Property.PropertyType; } }

        public PropertyImplement(PropertyInfo property)
            : base(property)
        {
        }

        protected override void CreateExpression()
        {
            var propertyInfo = Property;
            if (propertyInfo.CanRead)
            {
                var instance = Expression.Parameter(typeof(object), "instance");
                var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? null :
                    Expression.Convert(instance, propertyInfo.ReflectedType);
                var propertyAccess = Expression.Property(instanceCast, propertyInfo);
                var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));
                var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instance);
                _getter = lambda.Compile();
            }
            if (propertyInfo.CanWrite)
            {
                _setter = new MethodImplement(propertyInfo.GetSetMethod(true));
            }
        }

        public void SetValue(object instance, object value)
        {
            if (_setter != null)
                _setter.Invoke(instance, value);
        }

        public object GetValue(object instance)
        {
            if (_getter != null)
                return _getter(instance);
            return null;
        }
    }
}
