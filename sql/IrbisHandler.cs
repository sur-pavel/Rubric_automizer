using System;
using System.Collections.Generic;
using System.Linq;
using ManagedClient;


namespace RubricAutimatization
{

    public class IrbisHandler
    {
        private ManagedClient64 client = new ManagedClient64();
        internal bool connected = false;
        public IrbisHandler()
        {

        }

        internal void Connect()
        {
            try
            {
                if (connected)
                {
                    Disconnect();
                }
                client.ParseConnectionString("host=127.0.0.1;port=8888; user=a; password=1;");
                client.Connect();
                client.PushDatabase("MPDA");
                connected = true;
                Console.WriteLine("Connected!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "Error!");
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
                Console.WriteLine("ERROR DURING DISCONNECTION!");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.ToString());
            }
        }

    }


}