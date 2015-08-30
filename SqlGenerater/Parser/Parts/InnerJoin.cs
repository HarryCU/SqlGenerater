namespace SqlGenerater.Parser.Parts
{
    public class InnerJoin : Join
    {
        public override SqlPartType PartType
        {
            get { return SqlPartType.InnerJoin; }
        }

        public InnerJoin(TableBase refrence, TableBase table, Expression condition)
            : base(refrence, table, condition)
        {
        }
    }
}
