using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;

namespace BenchmarkNoSQL
{
    public class Redis : IDatabase
    {
        ConnectionMultiplexer client;
        StackExchange.Redis.IDatabase db;
        static Object m_lock = new Object();
        Random m_random;

        public Redis()
        {
            this.client = ConnectionMultiplexer.Connect("localhost,allowAdmin=true");
            this.db = client.GetDatabase();
            this.m_random = new Random(DateTime.Now.Millisecond);
        }

        public bool insert(Tick tick, Action callback = null)
        {
            double timestamp = tick.Time.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

            try
            {
                // Partie aleatoire pour que chaque clef soit unique (cas où plusieurs ticks par millisecondes)
                db.SortedSetAdd(tick.Symbol + "_ask", timestamp.ToString() + m_random.Next(0, 99999) + m_random.Next(0, 99999), (double)tick.Ask);
                db.SortedSetAdd(tick.Symbol + "_bid", timestamp.ToString() + m_random.Next(0, 99999) + m_random.Next(0, 99999), (double)tick.Bid);
            }
            catch (Exception e)
            {
                Console.WriteLine("Insertion failed -> "+e.Message);
                return false;
            }

            return true;
        }

        public void close()
        {
            this.client.Close();
            this.client.Dispose();
        }

        public void dropAll()
        {
            this.client.GetServer("localhost:6379").FlushAllDatabases();
        }
        
        public long count(string symbol)
        {
            return this.db.SortedSetLength(symbol + "_ask");
        }
    }
}
