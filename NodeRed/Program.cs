using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    public static async Task Main(string[] args)
    {
        using (ClientWebSocket clientWebSocket = new ClientWebSocket())
        {
            Uri serverUri = new Uri("wss://113.161.84.132:8081/FMA");

            try
            {
                await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);

                await ReceiveMessages(clientWebSocket);

                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect", CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static async Task ReceiveMessages(ClientWebSocket clientWebSocket)
    {
        byte[] buffer = new byte[1024];
        
        while (clientWebSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = null;
            try
            {
                result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recieve: {ex.Message}");
                break;
            }

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Message: {message}");
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("Close.");
                break;
            }
        }
    }
}
