using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace BenchmarkNoSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Count() > 0)
                    Configuration.ITERATIONS = long.Parse(args[0]);

                if (args.Count() > 1)
                    Configuration.SCENARIO = args[1];

                // Initialise la base de données
                IDatabase db = DBFactory.getDBInstance(Configuration.DB);

                List<Benchmark> list = new List<Benchmark>();

                // Lance les benchmarks de lecture
                for (int i = 0; i < Configuration.symbols.Length; i++)
                    list.Add(new Benchmark(Configuration.symbols[i], new Random().Next(0, 3), DBFactory.getDBInstance(Configuration.DB), false));

                // Attend que tous les benchmarks d'écriture soient terminés
                for (int i = 0; i < list.Count; i++)
                    list[i].m_Thread.Join();

                if (Configuration.SCENARIO == "READ")
                {
                    list = new List<Benchmark>();
                    Configuration.totalElapsed = 0;

                    // Lance les benchmarks de lecture
                    for (int i = 0; i < Configuration.symbols.Length; i++)
                        list.Add(new Benchmark(Configuration.symbols[i], new Random().Next(0, 3), DBFactory.getDBInstance(Configuration.DB), true));

                    // Attend que tous les benchmarks de lecture soient terminés
                    for (int i = 0; i < list.Count; i++)
                        list[i].m_Thread.Join();
                }

                for (int i = 0; i < list.Count; i++)
                {
                    Console.WriteLine();
                    Console.WriteLine("Benchmark " + list[i].m_symbol);
                    Console.WriteLine("Integrity check: " + db.count(list[i].m_symbol));
                    list[i].m_db.close();
                }

                db.dropAll();
                db.close();

                if (Configuration.SCENARIO == "WRITE")
                    Console.WriteLine(Configuration.totalDone + " inserted in " + Configuration.totalElapsed + " ms");
                else if (Configuration.SCENARIO == "READ")
                    Console.WriteLine("Read operation on " + Configuration.totalDone + " done in " + (double)Configuration.totalElapsed / (double)(100 * Configuration.symbols.Length) + " ms");
                    Console.WriteLine("Read operation on " + Configuration.totalDone + " done in " + (double)Configuration.totalElapsed / (double)Configuration.symbols.Length + " ms");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
