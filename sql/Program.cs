using System;

namespace RubricAutimatization
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            IrbisHandler irbisHandler = new IrbisHandler();
            irbisHandler.Connect();
            Console.ReadKey();
            irbisHandler.Disconnect();
            Environment.Exit(0);
        }
    }
}
