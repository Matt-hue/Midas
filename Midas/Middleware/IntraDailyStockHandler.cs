using System.Net.WebSockets;

namespace Midas.Middleware
{
    public class IntraDailyStockHandler
    {
        private readonly WebSocket _webSocket;

        public IntraDailyStockHandler(WebSocket webSocket)
        {
            _webSocket = webSocket;
        }
        public async Task RunAsync()
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await _webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                await _webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await _webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await _webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

    }
}
