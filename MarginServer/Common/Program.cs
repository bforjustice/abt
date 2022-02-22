namespace MarginServer
{
    using Exceptions;

    internal class Program
    {
        private static MainLogic mMainLogic;

        private static void Main(string[] args)
        {
            AssertDetector_Server.getInstance().addUnhandledExceptionHandler();

            mMainLogic = MainLogic.getInstance();
            mMainLogic.startLogic();
        }
    }
}