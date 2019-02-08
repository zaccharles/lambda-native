namespace LambdaNative.Example
{
    public static class EntryPoint
    {
        public static void Main()
        {
            LambdaNative.Run<Handler, Request, Response>();
        }
    }
}
