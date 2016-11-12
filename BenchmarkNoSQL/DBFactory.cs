using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkNoSQL
{
    public class DBFactory
    {
        public static IDatabase getDBInstance(string DBName)
        {
            if (DBName == "MongoDB")
                return new MongoDB();

            if (DBName == "Redis")
                return new Redis();

            if (DBName == "Cassandra")
                return new Cassandra();

            if (DBName == "CouchDB")
                return new CouchDB();

            return null;
        }
    }
}
