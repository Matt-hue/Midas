using System.Net.WebSockets;

namespace Midas.Middleware
{
    public class SocketWare
    {
        private RequestDelegate next;
        public SocketWare(RequestDelegate _next)
        {
            this.next = _next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await RunAsync(socket);
        }
        private async Task RunAsync(WebSocket socket)
        {
            try
            {
                var client = new IntraDailyStockHandler(socket);
                await client.RunAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
