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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using SqlGenerater.Reflection.Implements;
using SqlGenerater.Utils;

namespace SqlGenerater.Reflection
{
    public static class ReflectionHelper
    {
        private static readonly Locker Locker = new Locker();
        public const BindingFlags DefBindingFlags = BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance;
        public const BindingFlags AllBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        #region Property

        private static readonly IDictionary<string, IProperty> Properties = new Dictionary<string, IProperty>();

        public static IProperty GetProperty(PropertyInfo propertyInfo)
        {
            IProperty p = null;
            Locker.Lock();
            if (propertyInfo.ReflectedType != null)
            {
                var propertyName = string.Concat(propertyInfo.ReflectedType.FullName, propertyInfo.Name);
                if (!Properties.ContainsKey(propertyName))
                    Properties.Add(propertyName, new PropertyImplement(propertyInfo));
                p = Properties[propertyName];
            }
            Locker.Unlock();
            return p;
        }

        public static IProperty GetProperty(Type type, string propertyName, BindingFlags flag = DefBindingFlags)
        {
            IProperty prop;
            Locker.Lock();
            var propertyCurrentName = string.Concat(type.FullName, propertyName);
            if (!Properties.TryGetValue(propertyCurrentName, out prop))
            {
                var propertyInfo = type.GetProperty(propertyName, flag);
                if (propertyInfo != null)
                {
                    prop = new PropertyImplement(propertyInfo);
                    Properties.Add(propertyCurrentName, prop);
                }
            }
            Locker.Unlock();
            return prop;
        }

        public static ICollection<IProperty> AllProperty(Type type, BindingFlags flag = DefBindingFlags)
        {
            var properties = type.GetProperties(flag);
            return properties.Select(propertyInfo => new PropertyImplement(propertyInfo)).ToList<IProperty>();
        }

        public static bool HasProperty(Type type, string propertyName, BindingFlags flag)
        {
            var info = type.GetProperty(propertyName, flag);
            return info != null;
        }

        #endregion

        #region Method

        private static readonly IDictionary<string, IMethod> Methods = new Dictionary<string, IMethod>();

        public static IMethod GetMethodWithoutCache(MethodInfo methodInfo)
        {
            IMethod m = null;
            try
            {
                m = new MethodImplement(methodInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return m;
        }

        public static IMethod GetMethod(MethodInfo methodInfo)
        {
            IMethod m = null;
            Locker.Lock();
            if (methodInfo.ReflectedType != null)
            {
                var methodName = string.Concat(methodInfo.ReflectedType.FullName, methodInfo.Name);
                if (!Methods.ContainsKey(methodName))
                    Methods.Add(methodName, new MethodImplement(methodInfo));
                m = Methods[methodName];
            }
            Locker.Unlock();
            return m;
        }

        public static IMethod GetMethod(Type type, string methodName, BindingFlags flag = DefBindingFlags)
        {
            return GetMethod(type, methodName, Type.EmptyTypes, flag);
        }

        public static IMethod GetMethod(Type type, string methodName, Type[] parameters, BindingFlags flag = DefBindingFlags)
        {
            IMethod method;
            Locker.Lock();
            var methodCurrentName = string.Concat(type.FullName, methodName, (parameters != null ? parameters.Length.ToString(CultureInfo.InvariantCulture) : string.Empty));
            if (!Methods.TryGetValue(methodCurrentName, out method))
            {
                var methodInfo = type.GetMethod(methodName, flag, null, parameters, null);
                if (methodInfo != null)
                {
                    if (!Methods.ContainsKey(methodCurrentName))
                    {
                        method = new MethodImplement(methodInfo);
                        Methods.Add(methodCurrentName, method);
                    }
                    else
                    {
                        method = Methods[methodCurrentName];
                    }
                }
            }
            Locker.Unlock();
            return method;
        }

        public static bool HasMethod(Type type, string methodName, BindingFlags flag = DefBindingFlags)
        {
            var info = type.GetMethod(methodName, flag);
            return info != null;
        }

        #endregion

        #region Field

        private static readonly IDictionary<string, IField> Fields = new Dictionary<string, IField>();

        public static IField GetField(FieldInfo fieldInfo)
        {
            IField f = null;
            Locker.Lock();
            if (fieldInfo.ReflectedType != null)
            {
                var fieldName = string.Concat(fieldInfo.ReflectedType.FullName, fieldInfo.Name);
                if (!Fields.ContainsKey(fieldName))
                    Fields.Add(fieldName, new FieldImplement(fieldInfo));
                f = Fields[fieldName];
            }
            Locker.Unlock();
            return f;
        }

        public static IField GetField(Type type, string fieldName, BindingFlags flag = DefBindingFlags)
        {
            IField field;
            Locker.Lock();
            var fieldCurrentName = string.Concat(type.FullName, fieldName);
            if (!Fields.TryGetValue(fieldCurrentName, out field))
            {
                var fieldInfo = type.GetField(fieldName, flag);
                if (fieldInfo != null)
                {
                    if (Fields.ContainsKey(fieldCurrentName))
                    {
                        field = new FieldImplement(fieldInfo);
                        Fields.Add(fieldCurrentName, field);
                    }
                    else
                    {
                        field = Fields[fieldCurrentName];
                    }
                }
            }
            Locker.Unlock();
            return field;
        }

        #endregion

        #region Ctor

        private static readonly IDictionary<string, IConstructor> Constructors = new Dictionary<string, IConstructor>();

        public static IConstructor GetConstructor(Type type, Type[] types)
        {
            IConstructor constructor;
            Locker.Lock();
            var builder = new StringBuilder();
            {
                foreach (var item in types)
                    builder.Append(item.FullName);
                var constructorCurrentName = string.Concat(type.FullName, builder.ToString());


                if (!Constructors.TryGetValue(constructorCurrentName, out constructor))
                {
                    var constructorInfo = type.GetConstructor(types);
                    if (constructorInfo != null)
                    {
                        if (!Constructors.ContainsKey(constructorCurrentName))
                        {
                            constructor = new ConstructorImplement(constructorInfo);
                            Constructors.Add(constructorCurrentName, constructor);
                        }
                        else
                            constructor = Constructors[constructorCurrentName];
                    }
                }
            }
            builder.Clear();
            Locker.Unlock();
            return constructor;
        }

        #endregion

        #region Attribute

        public static IEnumerable<Attribute> GetAllAttribute(MemberInfo member)
        {
            return member.GetCustomAttributes(true).Cast<Attribute>();
        }

        public static T GetAttributeOne<T>(MemberInfo member)
        {
            var attrs = member.GetCustomAttributes(typeof(T), true);
            if (attrs.Length > 0)
                return (T)attrs[0];
            return default(T);
        }

        public static IEnumerable<T> GetAttribute<T>(MemberInfo member)
        {
            var attrs = member.GetCustomAttributes(typeof(T), true);
            if (attrs.Length > 0)
                foreach (T attr in attrs)
                {
                    yield return attr;
                }
        }

        public static bool HasAttribute<T>(MemberInfo member)
        {
            var attrs = member.GetCustomAttributes(typeof(T), true);
            return attrs.Length > 0;
        }

        public static T GetAttributeOne<T>(Assembly assembly)
        {
            var attrs = assembly.GetCustomAttributes(typeof(T), true);
            if (attrs.Length > 0)
                return (T)attrs[0];
            return default(T);
        }

        #endregion
    }
}
