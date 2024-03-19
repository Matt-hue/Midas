using Mapster;
using Midas.Data.Entities;

namespace Midas.Models
{
    public class TwelveDataTimeSeriesDto : IRegister
    {
        public TwelveDataMetaDto? Meta { get; set; }
        public IEnumerable<TwelveDataCandleDto>? Values { get; set; }
        public string Status { get; set; } = string.Empty;

        public void Register(TypeAdapterConfig config)
        {
            //config.ForType<TwelveDataCandleDto, Candle>()
            //    .Map(dest => dest.Symbol, src => src.);
        }
    }

    public class TwelveDataMetaDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Exchange_timezone { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string Mic_code { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class TwelveDataCandleDto
    {
        public string DateTime { get; set; } = string.Empty;
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }
}
