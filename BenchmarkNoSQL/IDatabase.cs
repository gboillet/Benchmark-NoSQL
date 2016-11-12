using System;
using System.Collections.Generic;

namespace BenchmarkNoSQL
{
    public interface IDatabase
    {
        bool insert(Tick tick, Action callback = null);
        void close();
        void dropAll();
        long count(string symbol);
    }
}
