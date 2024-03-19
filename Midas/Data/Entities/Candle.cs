namespace Midas.Data.Entities
{
    public class Candle
    {
        public int Id { get; set; }

        public string Interval { get; set; } = string.Empty;
        public DateTime DateTimeLocal { get; set; }
        public string TimeZoneLocal { get; set; } = string.Empty;
        public DateTime DateTimeUTC { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }

        //public string Exchange { get; set; } = string.Empty; //As in where it's traded
        //public string MicCode { get; set; } = string.Empty;
        //public string Type { get; set; } = string.Empty;
        //public string DataSource { get; set; } = string.Empty; where data was sourced
    }
}
