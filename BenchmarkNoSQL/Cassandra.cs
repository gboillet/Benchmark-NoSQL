using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;

namespace BenchmarkNoSQL
{
    class Cassandra : IDatabase
    {
        Cluster cluster;
        ISession session;
        static Object m_lock = new Object();
        Random m_random;
        static bool isCreated = false;
        
        public Cassandra()
        {
            this.cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            Console.WriteLine("Connected to cluster: " + cluster.Metadata.ClusterName.ToString());
            this.session = cluster.Connect();
            this.m_random = new Random(DateTime.Now.Millisecond);

            lock (m_lock)
            {
                // Créer les tables une fois seulement
                if (!Cassandra.isCreated)
                    createTables();
            }
        }

        void createTables()
        {
            this.session.Execute("CREATE KEYSPACE tickData WITH replication = {'class':'SimpleStrategy', 'replication_factor':1};");
            this.session.Execute("CREATE TABLE tickData.ticks (" +
            "id uuid," +
            "timestamp double," +
            "symbol text, " +
            "bid decimal," +
            "ask decimal," +
            "PRIMARY KEY (id, symbol));");

            Cassandra.isCreated = true;
        }
        
        public bool insert(Tick tick, Action callback = null)
        {
            double timestamp = tick.Time.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            
            // Pour l'UUID
            byte[] b = new byte[16];
            m_random.NextBytes(b);

            String query = "INSERT INTO tickData.ticks " +
            "(id, timestamp, symbol, bid, ask) " +
            "VALUES (" + new Guid(b) + "," + timestamp.ToString().Replace(",", ".") + ", '" + tick.Symbol + "', " + tick.Bid.ToString().Replace(",", ".") + ", " + tick.Ask.ToString().Replace(",", ".") + ");";

            session.Execute(query);
            
            return true;
        }

        public long count(string symbol)
        {
            RowSet results = session.Execute("SELECT COUNT(*) FROM tickData.ticks WHERE symbol ='"+symbol+"' ALLOW FILTERING;");

            foreach (Row row in results.GetRows())
            {
               if(row.GetValue<Int64>(0) > 0)
                   return (long)row.GetValue<Int64>(0);
            }

            return -1;
        }

        public void close()
        {
            this.cluster.Shutdown();
        }

        public void dropAll()
        {
            this.session.Execute("DROP TABLE tickData.ticks;");
            this.session.Execute("DROP KEYSPACE tickData;");
        }
    }
}
