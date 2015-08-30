using System.Reflection;

namespace SqlGenerater
{
    public class SqlParameter
    {
        public SqlParameter(string key, object value, MemberInfo member)
        {
            Key = key;
            Value = value;
            Member = member;
        }

        public string Key
        {
            get;
            private set;
        }

        public object Value
        {
            get;
            private set;
        }

        public MemberInfo Member
        {
            get;
            private set;
        }
    }
}
