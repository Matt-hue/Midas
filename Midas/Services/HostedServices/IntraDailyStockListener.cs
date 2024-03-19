using Microsoft.AspNetCore.SignalR.Client;

namespace Midas.Services.HostedServices
{
    public class IntraDailyStockListener : IHostedService, IAsyncDisposable
    {
        private HubConnection? hubConnection;
        private List<string> _messages = new ();

        public async ValueTask DisposeAsync()
        {
            await hubConnection!.DisposeAsync();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var url = "wss://ws.twelvedata.com/v1/quotes/price?apikey=976fccf07a6044dda3f3209efb205270";
            hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                _messages.Add(encodedMsg);
            });


            await hubConnection.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await hubConnection!.StopAsync(cancellationToken);
        }
    }
}
