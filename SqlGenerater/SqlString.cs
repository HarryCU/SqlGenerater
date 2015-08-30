using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SqlGenerater
{
    [DebuggerDisplay("{DebugView}")]
    public class SqlString
    {
        private readonly StringBuilder _builder;
        private readonly List<SqlParameter> _parameters;

        internal string DebugView
        {
            get { return ToString(); }
        }

        public SqlString()
        {
            _builder = new StringBuilder();
            _parameters = new List<SqlParameter>();
        }

        public IReadOnlyCollection<SqlParameter> Parameters
        {
            get { return _parameters.AsReadOnly(); }
        }

        internal void Append(string text, params object[] args)
        {
            _builder.AppendFormat(text, args);
        }

        internal void AddParameter(MemberInfo member, string key, object value)
        {
            _parameters.Add(new SqlParameter(key, value, member));
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
