using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BenchmarkNoSQL
{
    class MongoDB : IDatabase
    {
        string connectionString = "mongodb://localhost:27017";
        MongoClient client;
        MongoServer server;
        MongoDatabase database;

        public MongoDB()
        {
            this.client = new MongoClient(connectionString);
            this.server = client.GetServer();
            this.database = server.GetDatabase("tickData");
        }

        public bool insert(Tick tick, Action callback = null)
        {
            MongoCollection<Tick> collection = database.GetCollection<Tick>(tick.Symbol);

            collection.Insert(tick);

            return true;
        }

        public void printAll()
        {
            var collList = database.GetCollectionNames();
            MongoCollection<Tick> collection;

            foreach(string coll in collList)
            {
                // N'itère pas sur les tables system
                if (coll != "system.indexes")
                {
                    collection = database.GetCollection<Tick>(coll);

                    foreach (Tick t in collection.FindAll())
                        Console.WriteLine(t);
                }
            }
        }

        public void print(string symbol)
        {
            MongoCollection<Tick> collection = database.GetCollection<Tick>(symbol);

            foreach (Tick t in collection.FindAll())
                Console.WriteLine(t);
        }

        public long count(string symbol)
        {
            return database.GetCollection<Tick>(symbol).Count();
        }

        public void close()
        {
        }

        public void dropAll()
        {
            server.DropDatabase("tickData");
        }
    }
}
