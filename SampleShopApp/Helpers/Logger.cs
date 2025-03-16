namespace SampleShopApp.Helpers
{

    public interface ILogger
    {
        public abstract void Debug(string message);
        public abstract void Info(string message);
        public abstract void Warn(string message);
        public abstract void Error(string message);

    }

    public class ConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }
    }

    public static class GlobalLogger
    {
        public static ILogger logger { get; set; }
    }
}
