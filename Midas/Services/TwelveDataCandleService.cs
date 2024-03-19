using CsvHelper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Midas.Data;
using Midas.Data.Entities;
using Midas.Models;
using Midas.Models.Csv;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;

namespace Midas.Services
{
    public class TwelveDataCandleService : ITwelveDataCandleService, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private IHttpClientFactory _httpClientFactory;

        private HttpClient _httpClient;

        public TwelveDataCandleService(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;

            _httpClient = _httpClientFactory.CreateClient();
        }

        public async Task<CandleDto> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Candles.GetByIdAsync(id);

            var responseDto = entity.Adapt<CandleDto>();

            return responseDto;
        }

        public async Task<IEnumerable<CandleDto>> FindAsync(string? symbol, string? timeInterval, int size = 100)
        {

            Expression<Func<Candle, bool>> filterPredicate = new FilterPredicateBuilder()
                .IncludeSymbol(symbol)
                .IncludeTimeInterval(timeInterval);


            var entities = _unitOfWork.Candles.QueryableFind(filterPredicate).Take(size);

            var result = await entities.ToListAsync();

            var responses = result.Adapt<IEnumerable<CandleDto>>() ?? new List<CandleDto>();

            return responses;
        }

        public async Task CreateRangeAsync(Dictionary<string, TwelveDataTimeSeriesDto> dict)
        {
            var entities = new List<Candle>();
            foreach (string key in dict.Keys)
            {
                var dto = dict[key];
                //IEnumerable<TwelveDataCandleDto> candleDtos = index.Values ?? throw new NullReferenceException();

                //string symbol = index?.Meta?.Symbol ?? throw new NullReferenceException();
                //string interval = index?.Meta?.Interval ?? throw new NullReferenceException();

                //var tz = !string.IsNullOrEmpty(index.Meta?.Exchange_timezone) ? index.Meta?.Exchange_timezone : throw new NullReferenceException();
                //var tzi = TimeZoneInfo.FindSystemTimeZoneById(tz);

                //foreach (TwelveDataCandleDto candleDto in candleDtos)
                //{
                //    var localDateTime = DateTime.Parse(candleDto.DateTime);
                //    var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, tzi);

                //    var entity = candleDto.Adapt<Candle>();

                //    entity.DateTimeLocal = localDateTime;
                //    entity.TimeZoneLocal = tzi.StandardName;
                //    entity.DateTimeUTC = utcDateTime;
                //    entity.Symbol = symbol;
                //    entity.Interval = interval;

                //    entities.Add(entity);
                //}

                entities.AddRange(MapTimeSeriesToEntities(dto));
            }

            await _unitOfWork.Candles.AddRangeAsync(entities);
            await _unitOfWork.CompleteAsync();

        }

        public async Task CreateRangeAsync(TwelveDataTimeSeriesDto dto)
        {
            var entities = MapTimeSeriesToEntities(dto);

            await _unitOfWork.Candles.AddRangeAsync(entities);
            await _unitOfWork.CompleteAsync();
        }

        private static IEnumerable<Candle> MapTimeSeriesToEntities(TwelveDataTimeSeriesDto dto)
        {
            var entities = new List<Candle>();

            IEnumerable<TwelveDataCandleDto> candleDtos = dto.Values ?? throw new NullReferenceException();

            string symbol = dto?.Meta?.Symbol ?? throw new NullReferenceException();
            string interval = dto?.Meta?.Interval ?? throw new NullReferenceException();

            var tz = !string.IsNullOrEmpty(dto.Meta?.Exchange_timezone) ? dto.Meta.Exchange_timezone : throw new NullReferenceException();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(tz);

            foreach (TwelveDataCandleDto candleDto in candleDtos)
            {
                var localDateTime = DateTime.Parse(candleDto.DateTime);
                var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, tzi);

                var entity = candleDto.Adapt<Candle>();

                entity.DateTimeLocal = localDateTime;
                entity.TimeZoneLocal = tzi.StandardName;
                entity.DateTimeUTC = utcDateTime;
                entity.Symbol = symbol;
                entity.Interval = interval;

                entities.Add(entity);
            }

            return entities;
        }

        public async Task SeedHistoricalDataAsync(string? symbols = null, int outputsize = 5000)
        {

            var key = "976fccf07a6044dda3f3209efb205270";
            if(symbols == null)
            {
                symbols = "AMD, NVDA, DJI";
            }
            var interval = "1day";
            var uri = $"https://api.twelvedata.com/time_series?symbol={symbols}&interval={interval}&outputsize={outputsize}&apikey={key}";

            try
            {
                var stocks = await _httpClient.GetFromJsonAsync<Dictionary<string, TwelveDataTimeSeriesDto>>(uri);

                if (stocks != null && !stocks.Any(x => x.Value.Status.Contains("error")))
                {
                    await CreateRangeAsync(stocks);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SeedLatestHistoricalDataAsync(string symbol, string interval = "1day")
        {
            DateTime? latest = await _unitOfWork.Candles
                .QueryableFind(x => x.Symbol == symbol && x.Interval == interval)
                .MaxAsync(x => (DateTime?)x.DateTimeLocal);

            if (latest.HasValue)
            {
                switch (interval)
                {
                    case "1day":
                        latest = latest.Value.AddDays(1);
                        break;
                    case "1h":
                        latest = latest.Value.AddHours(1);
                        break;
                    case "30min":
                        latest = latest.Value.AddMinutes(30);
                        break;
                    case "5min":
                        latest = latest.Value.AddMinutes(5);
                        break;
                    default:
                        break;
                }

                // todo: improve
                // prevent adding unfinished candles
                if (latest >= DateTime.UtcNow.AddDays(-2))
                {
                    return;
                }
            }

            var stocks = await PullLatestTimeSeriesDataAsync(symbol, interval, latest);

            if(stocks != null && !stocks.Status.Contains("error"))
            {
                await CreateRangeAsync(stocks);
            }
        }

        private async Task<TwelveDataTimeSeriesDto?> PullLatestTimeSeriesDataAsync(
            string symbol,
            string interval,
            DateTime? startDate = null,
            int outputsize = 5000
            )
        {
            var key = "976fccf07a6044dda3f3209efb205270";

            var uri = $"https://api.twelvedata.com/time_series?symbol={symbol}&interval={interval}&outputsize={outputsize}&apikey={key}";
            if (startDate.HasValue)
            {
                uri += $"&start_date={startDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}";
            }

            try
            {
                var stocks = await _httpClient.GetFromJsonAsync<TwelveDataTimeSeriesDto>(uri);
                return stocks;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SeedHistoricalDataLocallyAsync()
        {
            using var sr = new StreamReader(@"C:\Users\MatteoCatrini\Downloads\^AEX.csv");

            using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
            {
                string symbol = "^AEX";
                string interval = "1day";
                var tzi = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

                IEnumerable<CandleCsvModel> records = csv.GetRecords<CandleCsvModel>().ToList();

                var entities = new List<Candle>();
                foreach (var r in records)
                {
                    var entity = new Candle();
                    entity.Symbol = symbol;
                    entity.TimeZoneLocal = tzi.StandardName;
                    entity.Interval = interval;

                    var localDateTime = DateTime.Parse(r.Date);
                    var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, tzi);

                    entity.DateTimeLocal = localDateTime;
                    entity.DateTimeUTC = utcDateTime;

                    Double.TryParse(r.Open, out double open);
                    entity.Open = open;

                    Double.TryParse(r.High, out double high);
                    entity.High = high;

                    Double.TryParse(r.Low, out double low);
                    entity.Low = low;

                    Double.TryParse(r.Close, out double close);
                    entity.Close = close;

                    Double.TryParse(r.Volume, out double volume);
                    entity.Volume = volume;

                    entities.Add(entity);
                }

                await _unitOfWork.Candles.AddRangeAsync(entities);
                await _unitOfWork.CompleteAsync();
            }
        }
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
