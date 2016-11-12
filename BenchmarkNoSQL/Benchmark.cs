using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace BenchmarkNoSQL
{
    public class Benchmark
    {
        public Thread m_Thread;
        public string m_symbol;
        public IDatabase m_db;
        int m_frequency;
        Random m_random;
        
        public Benchmark(string symbol, int frequency, IDatabase db, bool isRead)
        {
            this.m_symbol = symbol;
            this.m_frequency = frequency;
            this.m_random = new Random(DateTime.Now.Millisecond);
            this.m_db = db;

            if(isRead)
                m_Thread = new Thread(read);
            else
                m_Thread = new Thread(write);

            m_Thread.Start();
        }

        void write()
        {
            Tick t;
            Stopwatch watch;
            long delay = m_random.Next(200, 2000); // Le délai de X ms apparait tous les 200 à 2000 ticks

            for (long i=0; i<Configuration.ITERATIONS; i++)
            {
                t = new Tick();
                t.Symbol = m_symbol;
                t.Time = DateTime.Now;
                t.Bid = Math.Round(Configuration.MINPRICE + ((decimal)m_random.NextDouble() * (Configuration.MAXPRICE - Configuration.MINPRICE)), 2);
                t.Ask = t.Bid + 0.5m;

                watch = Stopwatch.StartNew();

                m_db.insert(t);

                watch.Stop();

                lock (Configuration.elapsedLock)
                {
                    Configuration.totalElapsed += watch.ElapsedMilliseconds;
                    Configuration.totalDone++;
                }

                // Retard insere tous les X ticks uniquement quand on est en scénario écriture
                if(Configuration.SCENARIO == "WRITE" && i%delay == 0)
                    Thread.Sleep(m_frequency);
            }
        }

        // Scenario lecture: 100 requetes et moyenne du temps d'accès
        void read()
        {
            Stopwatch watch;

            for (int i = 0; i < 100; i++)
            {
                watch = Stopwatch.StartNew();
                
                m_db.count(this.m_symbol);
                
                watch.Stop();

                lock (Configuration.elapsedLock)
                {
                    Configuration.totalElapsed += watch.ElapsedMilliseconds;
                }
            }
        }
    }
}
