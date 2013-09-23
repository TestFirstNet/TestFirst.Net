namespace TestFirst.Net.Util
{
    public static class InserterUtil
    {
        public static IInserter GetRootInserter(IInserter inserter)
        {
            while (inserter is IReturnParentInserter)
            {
                inserter = (inserter as IReturnParentInserter).GetParentInserter();
            }
            return inserter;
        }
    }
}
