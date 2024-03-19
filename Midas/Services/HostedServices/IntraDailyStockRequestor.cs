using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Midas.Models;
using System.Net.Http;
using System.Text.Json;

namespace Midas.Services.HostedServices
{
    public class IntraDailyStockRequestor : BackgroundService//, IDisposable
    {
        private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromSeconds(2));

        private ILogger<IntraDailyStockRequestor> _logger;
        private IHttpClientFactory _httpClientFactory;

        //private ITwelveDataCandleService _dataCandleService;
        private IServiceProvider _services;
        public IntraDailyStockRequestor(IHttpClientFactory httpClientFactory, ILogger<IntraDailyStockRequestor> logger, IServiceProvider services)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using HttpClient client = _httpClientFactory.CreateClient();

            var key = "976fccf07a6044dda3f3209efb205270";
            var symbols = "AMD, NVDA";//"AMD,NVDA,EUR/USD";
            var interval = "1day"; //1min, 5min, 15min, 30min, 45min, 1h, 2h, 4h, 1day, 1week, 1month
            var outputsize = 5000;//5000;
            var uri = $"https://api.twelvedata.com/time_series?symbol={symbols}&interval={interval}&outputsize={outputsize}&apikey={key}";

            while (await _periodicTimer.WaitForNextTickAsync(stoppingToken)
                && !stoppingToken.IsCancellationRequested
                && false) 
            {
                try
                {
                    var stocks = await client.GetFromJsonAsync<Dictionary<string, TwelveDataTimeSeriesDto>>(
                        uri,
                        new JsonSerializerOptions(JsonSerializerDefaults.Web));
                    //var stocks = await client.GetFromJsonAsync<dynamic>(
                    //uri,
                    //new JsonSerializerOptions(JsonSerializerDefaults.Web));

                    if (stocks != null)
                    {
                        using (var scope = _services.CreateScope())
                        {
                            var twelveDataCandleService =
                                scope.ServiceProvider
                                    .GetRequiredService<ITwelveDataCandleService>();
                            await twelveDataCandleService.CreateRangeAsync(stocks);
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError("Stock data retrieval issue: {Error}", ex);
                }
            }

        }
    }
}
