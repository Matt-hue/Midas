using CsvHelper.Configuration.Attributes;
using Mapster;
using Midas.Data.Entities;
//using FileHelpers;

namespace Midas.Models.Csv
{

    //[DelimitedRecord(",")]
    public class CandleCsvModel : IRegister
    {
        //[Name("Date")]
        //[FieldConverter(ConverterKind.Date, "dd-MM-yyyy")]
        public string? Date { get; set; }
        public string Open { get; set; }
        public string? Close { get; set; }
        public string? High { get; set; }
        public string? Low { get; set; }
        [Name("Adj Close")]
        public string? AdjClose { get; set; }
        public string? Volume { get; set; }

        public void Register(TypeAdapterConfig config)
        {
            config.ForType<CandleCsvModel, Candle>()
                .Map(dest => dest.Open, src => Convert.ToDouble(src.Open));
        }
    }
}
