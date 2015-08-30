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
    public abstract class Disposer : IDisposable
    {
        private volatile bool _disposed;

        ~Disposer()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected bool Disposed
        {
            get
            {
                return _disposed;
            }
        }

        protected virtual bool ReleasePrevious()
        {
            return true;
        }

        protected abstract void Release();

        private void Dispose(bool disposing)
        {
            if (disposing && ReleasePrevious() && !_disposed)
            {
                Release();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
