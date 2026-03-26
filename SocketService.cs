using SocketIOClient;

namespace ShuttleTrackerApp.Services;

public class SocketService
{
    private static SocketIO? socket;

    public static SocketIO GetSocket()
    {
        if (socket == null)
        {
            socket = new SocketIO(new Uri("http://10.0.2.2:3000"));

            socket.OnConnected += (s, e) =>
            {
                Console.WriteLine("Connected to server");
            };

            socket.OnDisconnected += async (s, e) =>
            {
                Console.WriteLine("Disconnected. Reconnecting...");
                await Task.Delay(3000);

                try
                {
                    await socket.ConnectAsync();
                }
                catch
                {
                    Console.WriteLine("Reconnect failed");
                }
            };
        }

        return socket;
    }

    public static async Task ConnectAsync()
    {
        try
        {
            var s = GetSocket();

            if (!s.Connected)
                await s.ConnectAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Socket connect error: " + ex.Message);
        }
    }
}