using System.Net;

namespace Quotes_Generator
{
    public class GenerationConfig
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public IPAddress MulticastGroup { get; set; }
        public int MulticastPort { get; set; }
    }
}