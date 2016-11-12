using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Divan;
using Divan.Linq;

namespace BenchmarkNoSQL
{
    class CouchDB : IDatabase
    {
        static Object m_lock = new Object();
        Random m_random;
        CouchServer server;
        ICouchDatabase db;

        public CouchDB()
        {
            this.server = new CouchServer("localhost", 5984);
            this.db = server.GetDatabase("ticks");
            this.m_random = new Random(DateTime.Now.Millisecond);
        }

        public bool insert(Tick tick, Action callback = null)
        {
            Tick_CouchDB t = new Tick_CouchDB();
            t.Ask = tick.Ask;
            t.Bid = tick.Bid;
            t.Id = tick.Id;
            t.Symbol = tick.Symbol;
            t.Time = tick.Time;

            db.SaveDocument(t);

            return true;
        }

        public void close()
        {
           
        }

        public void dropAll()
        {
            this.server.DeleteAllDatabases();
        }
        
        public long count(string symbol)
        {
            ICouchViewDefinition tempView = db.NewTempView("ticks", "ticks", "if (doc.docType && doc.docType == 'tick') emit(doc.symbol, doc);");
            CouchLinqQuery<Tick> linqTicks = tempView.LinqQuery<Tick>();

            var ticks = from t in linqTicks where t.Symbol == symbol select t;
            int count = 0;

            foreach (var t in ticks)
                count++;

            db.DeleteDocument(tempView.Doc);

            return count;
        }
    }
}