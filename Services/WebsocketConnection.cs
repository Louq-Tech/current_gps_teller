using CommunityToolkit.Maui.Alerts;
using System.Net.WebSockets;

namespace Current_GPS_Teller
{
    public class WebsocketConnection
    {
        public ClientWebSocket Client { get; private set; }
        // public ClientWebSocket LocationClient { get; private set; }

        public WebsocketConnection()
        {
            Client = new ClientWebSocket();
            Client.Options.KeepAliveInterval = TimeSpan.FromMinutes(30);
        }

        public async Task WebsocketConnectingFunction()
        {
            try
            {
                await Client.ConnectAsync(new Uri("ws://in3.localto.net:8765"), CancellationToken.None);
            }
            catch (Exception ex)
            {
                await Toast.Make($"Connection Error: {ex.Message}", CommunityToolkit.Maui.Core.ToastDuration.Long, 10).Show();
            }
        }
    }
}