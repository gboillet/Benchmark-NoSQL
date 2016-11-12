using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkNoSQL
{
    class Configuration
    {
        public static long ITERATIONS = 5000;
        public static String SCENARIO = "READ";
        public const int FREQUENCY = 1;
        public const int MAXPRICE = 40;
        public const int MINPRICE = 15;
        public const String DB = "Redis";
        public static long totalElapsed = 0;
        public static long totalDone = 0;
        public static Object elapsedLock = new Object();
        public static string[] symbols = { "RXH4", "GLE.PA", "EURUSD", "USDGBP" };
    }
}
