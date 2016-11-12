using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Divan;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BenchmarkNoSQL
{
    public class Tick
    {
        public Tick()
        {}

        public ObjectId Id { get; set; }

        public string Symbol { get { return m_sSymbol; } set { m_sSymbol = value; } }
        string m_sSymbol;

        /// <summary>
        /// Bid (offer) price
        /// </summary>
        public decimal Bid { get { return m_dBid; } set { m_dBid = value; } }
        decimal m_dBid;

        /// <summary>
        /// Ask (demand) price
        /// </summary>
        public decimal Ask { get { return m_dAsk; } set { m_dAsk = value; } }
        decimal m_dAsk;
        
        /// <summary>
        /// Server time of the Tick
        /// </summary>
        public DateTime Time { get { return m_Time; } set { m_Time = value; } }
        DateTime m_Time;

        public override string ToString()
        {
            return m_Time + " - " + m_sSymbol + " bid=" + m_dBid + " ask=" + m_dAsk;
        }
    }
}
