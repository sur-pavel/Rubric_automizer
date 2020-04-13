using ManagedClient;
using System;

namespace Rubric_automizer
{
    internal class IrbisHandler
    {
        private ManagedClient64 client;

        public IrbisHandler()
        {
            try
            {
                client = new ManagedClient64();
                client.ParseConnectionString("host=127.0.0.1;port=8888; user=СПА;password=1;");
                client.Connect();
                client.PushDatabase("MPDA");
                Console.WriteLine("Connected to irbis_server successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal void Disconnect()
        {
            try
            {
                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal int GetMaxMfn()
        {
            return client.GetMaxMfn();
        }

        internal ShortRecord GetShortRecord(int mfn)
        {
            ShortRecord shortRecord = new ShortRecord();
            return shortRecord;
        }
    }
}