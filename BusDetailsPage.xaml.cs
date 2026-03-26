using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using SocketIOClient;
using System.Text.Json;

namespace ShuttleTrackerApp.Views;

public class BusDetailsPage : ContentPage
{
    Microsoft.Maui.Controls.Maps.Map map;
    int busId;
    SocketIO socket;

    Pin busPin; 

    public BusDetailsPage(int id)
    {
        busId = id;
        map = new Microsoft.Maui.Controls.Maps.Map
        {
            VerticalOptions = LayoutOptions.FillAndExpand
        };

        Content = map;

        Init();
    }

    async void Init()
    {
        socket = new SocketIO(new Uri("http://10.0.2.2:3000"));
        await socket.ConnectAsync();

        socket.On("busUpdate", async (response) =>
        {
            var json = response.ToString();
            if (string.IsNullOrEmpty(json)) return;

            using var doc = JsonDocument.Parse(json);

            int id = doc.RootElement.GetProperty("busId").GetInt32();
            if (id != busId) return;

            //   (define lat/lng properly)
            double lat = doc.RootElement.GetProperty("lat").GetDouble();
            double lng = doc.RootElement.GetProperty("lng").GetDouble();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var busLocation = new Location(lat, lng);

                if (busPin == null)
                {
                    busPin = new Pin
                    {
                        Label = $"Bus {busId}",
                        Location = busLocation
                    };

                    map.Pins.Add(busPin);
                }
                else
                {
                    busPin.Location = busLocation;
                }

                map.MoveToRegion(
                    MapSpan.FromCenterAndRadius(busLocation, Distance.FromKilometers(1)));
            });
        });
    }
}