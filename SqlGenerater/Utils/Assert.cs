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

namespace SqlGenerater.Utils
{
    internal sealed class Assert
    {
        public static void CheckArgument(object value, string argName)
        {
            if (value == null)
                throw new ArgumentNullException(argName);
        }

        public static T CheckNull<T>(T value)
        {
            if (value == null)
                throw new NullReferenceException();
            return value;
        }

        public static void CheckNullOrEmpty(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new StringNullOrEmptyException();
        }

        public static void NotSupport(string message, params object[] args)
        {
            throw new NotSupportedException(string.Format(message, args));
        }
    }
}
