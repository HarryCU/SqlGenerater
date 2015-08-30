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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

namespace SqlGenerater.Reflection
{
    /// <summary>
    /// Type related helper methods
    /// </summary>
    public static class TypeExtensions
    {
        public static object NewDef(this Type type)
        {
            return New(type, Type.EmptyTypes);
        }

        public static object New(this Type type, params object[] @params)
        {
            var paramTypes = new Type[@params.Length];
            for (int i = 0; i < @params.Length; i++)
            {
                paramTypes[i] = @params[i].GetType();
            }
            return New(type, paramTypes, @params);
        }

        public static object New(this Type type, Type[] paramTypes, params object[] @params)
        {
            var ctor = ReflectionHelper.GetConstructor(type, paramTypes);
            if (ctor != null)
                return ctor.Inovke(@params);
            return null;
        }

        public static Type FindIEnumerable(this Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }

        public static Type GetSequenceType(this Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(elementType);
        }

        public static Type GetElementType(this Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }

        public static bool IsNullableType(this Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullAssignable(this Type type)
        {
            return !type.IsValueType || IsNullableType(type);
        }

        public static Type GetNonNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public static Type GetNullAssignableType(this Type type)
        {
            if (!IsNullAssignable(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }

        public static ConstantExpression GetNullConstant(this Type type)
        {
            return Expression.Constant(null, GetNullAssignableType(type));
        }

        public static Type GetMemberType(this MemberInfo mi)
        {
            FieldInfo fi = mi as FieldInfo;
            if (fi != null) return fi.FieldType;
            PropertyInfo pi = mi as PropertyInfo;
            if (pi != null) return pi.PropertyType;
            EventInfo ei = mi as EventInfo;
            if (ei != null) return ei.EventHandlerType;
            MethodInfo meth = mi as MethodInfo;  // property getters really
            if (meth != null) return meth.ReturnType;
            return null;
        }

        public static object GetDefault(this Type type)
        {
            bool isNullable = !type.IsValueType || TypeExtensions.IsNullableType(type);
            if (!isNullable)
                return Activator.CreateInstance(type);
            return null;
        }

        public static bool IsReadOnly(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return (((FieldInfo)member).Attributes & FieldAttributes.InitOnly) != 0;
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)member;
                    return !pi.CanWrite || pi.GetSetMethod() == null;
                default:
                    return true;
            }
        }

        public static bool IsInteger(this Type type)
        {
            Type nnType = GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a type (or type's element type)
        /// instance can be null in the underlying data store.
        /// </summary>
        /// <param name="type">A <see cref="System.Type"/> instance. </param>
        /// <returns> True, if the type parameter is a closed generic nullable type; otherwise, False.</returns>
        /// <remarks>Arrays of Nullable types are treated as Nullable types.</remarks>
        public static bool IsNullable(this Type type)
        {
            while (type.IsArray)
                type = type.GetElementType();

            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Returns the underlying type argument of the specified type.
        /// </summary>
        /// <param name="type">A <see cref="System.Type"/> instance. </param>
        /// <returns><list>
        /// <item>The type argument of the type parameter,
        /// if the type parameter is a closed generic nullable type.</item>
        /// <item>The underlying Type if the type parameter is an enum type.</item>
        /// <item>Otherwise, the type itself.</item>
        /// </list>
        /// </returns>
        public static Type GetUnderlyingType(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (IsNullable(type))
                type = type.GetGenericArguments()[0];

            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            return type;
        }

        /// <summary>
        /// Determines whether the specified types are considered equal.
        /// </summary>
        /// <param name="parent">A <see cref="System.Type"/> instance. </param>
        /// <param name="child">A type possible derived from the <c>parent</c> type</param>
        /// <returns>True, when an object instance of the type <c>child</c>
        /// can be used as an object of the type <c>parent</c>; otherwise, false.</returns>
        /// <remarks>Note that nullable types does not have a parent-child relation to it's underlying type.
        /// For example, the 'int?' type (nullable int) and the 'int' type
        /// aren't a parent and it's child.</remarks>
        public static bool IsSameOrParent(this Type parent, Type child)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (child == null) throw new ArgumentNullException("child");

            if (parent == child ||
                child.IsEnum && Enum.GetUnderlyingType(child) == parent ||
                child.IsSubclassOf(parent))
            {
                return true;
            }

            if (parent.IsInterface)
            {
                Type[] interfaces = child.GetInterfaces();

                foreach (Type t in interfaces)
                    if (t == parent)
                        return true;
            }

            return false;
        }

        public static Type GetGenericType(this Type genericType, Type type)
        {
            if (genericType == null) throw new ArgumentNullException("genericType");

            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
                    return type;

                if (genericType.IsInterface)
                {
                    foreach (Type interfaceType in type.GetInterfaces())
                    {
                        Type gType = GetGenericType(genericType, interfaceType);

                        if (gType != null)
                            return gType;
                    }
                }

                type = type.BaseType;
            }

            return null;
        }

        ///<summary>
        /// Gets the Type of a list item.
        ///</summary>
        /// <param name="list">A <see cref="System.Object"/> instance. </param>
        ///<returns>The Type instance that represents the exact runtime type of a list item.</returns>
        public static Type GetListItemType(this object list)
        {
            Type typeOfObject = typeof(object);

            if (list == null)
                return typeOfObject;

            if (list is Array)
                return list.GetType().GetElementType();

            Type type = list.GetType();

            // object[] attrs = type.GetCustomAttributes(typeof(DefaultMemberAttribute), true);
            // string   itemMemberName = (attrs.Length == 0)? "Item": ((DefaultMemberAttribute)attrs[0]).MemberName;

            if (list is IList || list is ITypedList || list is IListSource)
            {
                PropertyInfo last = null;

                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (pi.GetIndexParameters().Length > 0 && pi.PropertyType != typeOfObject)
                    {
                        if (pi.Name == "Item")
                            return pi.PropertyType;

                        last = pi;
                    }
                }

                if (last != null)
                    return last.PropertyType;
            }

            try
            {
                if (list is IList)
                {
                    IList l = (IList)list;

                    for (int i = 0; i < l.Count; i++)
                    {
                        object o = l[i];

                        if (o != null && o.GetType() != typeOfObject)
                            return o.GetType();
                    }
                }
                else if (list is IEnumerable)
                {
                    foreach (object o in (IEnumerable)list)
                    {
                        if (o != null && o.GetType() != typeOfObject)
                            return o.GetType();
                    }
                }
            }
            catch
            {
            }

            return typeOfObject;
        }

        ///<summary>
        /// Gets the Type of a list item.
        ///</summary>
        /// <param name="listType">A <see cref="System.Type"/> instance. </param>
        ///<returns>The Type instance that represents the exact runtime type of a list item.</returns>
        public static Type GetListItemType(this Type listType)
        {
            if (listType.IsGenericType)
            {
                Type[] elementTypes = GetGenericArguments(listType, typeof(IList));

                if (elementTypes != null)
                    return elementTypes[0];
            }

            if (IsSameOrParent(typeof(IList), listType) ||
                IsSameOrParent(typeof(ITypedList), listType) ||
                IsSameOrParent(typeof(IListSource), listType))
            {
                Type elementType = listType.GetElementType();

                if (elementType != null)
                    return elementType;

                PropertyInfo last = null;

                foreach (PropertyInfo pi in listType.GetProperties())
                {
                    if (pi.GetIndexParameters().Length > 0 && pi.PropertyType != typeof(object))
                    {
                        if (pi.Name == "Item")
                            return pi.PropertyType;

                        last = pi;
                    }
                }

                if (last != null)
                    return last.PropertyType;
            }

            return typeof(object);
        }

        /// <summary>
        /// Gets a value indicating whether a type can be used as a db primitive.
        /// </summary>
        /// <param name="type">A <see cref="System.Type"/> instance. </param>
        /// <returns> True, if the type parameter is a primitive type; otherwise, False.</returns>
        /// <remarks><see cref="System.String"/>. <see cref="Stream"/>. 
        /// <see cref="XmlReader"/>. <see cref="XmlDocument"/>. are specially handled by the library
        /// and, therefore, can be treated as scalar types.</remarks>
        public static bool IsScalar(this Type type)
        {
            while (type.IsArray)
                type = type.GetElementType();

            return type.IsValueType
                || type == typeof(string)
#if FW3
                || type == typeof(System.Data.Linq.Binary)
#endif
 || type == typeof(Stream)
                || type == typeof(XmlReader)
                || type == typeof(XmlDocument);
        }

        ///<summary>
        /// Returns an array of Type objects that represent the type arguments
        /// of a generic type or the type parameters of a generic type definition.
        ///</summary>
        /// <param name="type">A <see cref="System.Type"/> instance.</param>
        ///<param name="baseType">Non generic base type.</param>
        ///<returns>An array of Type objects that represent the type arguments
        /// of a generic type. Returns an empty array if the current type is not a generic type.</returns>
        public static Type[] GetGenericArguments(this Type type, Type baseType)
        {
            string baseTypeName = baseType.Name;

            for (Type t = type; t != typeof(object) && t != null; t = t.BaseType)
                if (t.IsGenericType && (baseTypeName == null || t.Name.Split('`')[0] == baseTypeName))
                    return t.GetGenericArguments();

            foreach (Type t in type.GetInterfaces())
                if (t.IsGenericType && (baseTypeName == null || t.Name.Split('`')[0] == baseTypeName))
                    return t.GetGenericArguments();

            return null;
        }

        /// <summary>
        /// Substitutes the elements of an array of types for the type parameters
        /// of the current generic type definition and returns a Type object
        /// representing the resulting constructed type.
        /// </summary>
        /// <param name="type">A <see cref="System.Type"/> instance.</param>
        /// <param name="typeArguments">An array of types to be substituted for
        /// the type parameters of the current generic type.</param>
        /// <returns>A Type representing the constructed type formed by substituting
        /// the elements of <paramref name="typeArguments"/> for the type parameters
        /// of the current generic type.</returns>
        /// <seealso cref="System.Type.MakeGenericType"/>
        public static Type TranslateGenericParameters(this Type type, Type[] typeArguments)
        {
            // 'T paramName' case
            //
            if (type.IsGenericParameter)
                return typeArguments[type.GenericParameterPosition];

            // 'List<T> paramName' or something like that.
            //
            if (type.IsGenericType && type.ContainsGenericParameters)
            {
                Type[] genArgs = type.GetGenericArguments();

                for (int i = 0; i < genArgs.Length; ++i)
                    genArgs[i] = TranslateGenericParameters(genArgs[i], typeArguments);

                return type.GetGenericTypeDefinition().MakeGenericType(genArgs);
            }

            // Non-generic type.
            //
            return type;
        }

        public static bool CompareParameterTypes(this Type goal, Type probe)
        {
            if (goal == probe)
                return true;

            if (goal.IsGenericParameter)
                return CheckConstraints(goal, probe);
            if (goal.IsGenericType && probe.IsGenericType)
                return CompareGenericTypes(goal, probe);

            return false;
        }

        public static bool CheckConstraints(this Type goal, Type probe)
        {
            Type[] constraints = goal.GetGenericParameterConstraints();

            for (int i = 0; i < constraints.Length; i++)
                if (!constraints[i].IsAssignableFrom(probe))
                    return false;

            return true;
        }

        public static bool CompareGenericTypes(this Type goal, Type probe)
        {
            Type[] genArgs = goal.GetGenericArguments();
            Type[] specArgs = probe.GetGenericArguments();

            bool match = (genArgs.Length == specArgs.Length);

            for (int i = 0; match && i < genArgs.Length; i++)
            {
                if (genArgs[i] == specArgs[i])
                    continue;

                if (genArgs[i].IsGenericParameter)
                    match = CheckConstraints(genArgs[i], specArgs[i]);
                else if (genArgs[i].IsGenericType && specArgs[i].IsGenericType)
                    match = CompareGenericTypes(genArgs[i], specArgs[i]);
                else
                    match = false;
            }

            return match;
        }
    }
}
