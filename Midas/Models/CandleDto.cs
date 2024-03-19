namespace Midas.Models
{
    public class CandleDto
    {
        public int Id { get; set; }
        public string Interval { get; set; } = string.Empty;
        public DateTime DateTimeLocal { get; set; }
        public string TimeZoneLocal { get; set; } = string.Empty;
        public DateTime DateTimeUTC { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }
}
