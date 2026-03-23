using Microsoft.Maui.ApplicationModel;
using SocketIOClient;
using System.Net.Http.Json;
using System.Net.Sockets;

namespace ShuttleTrackerApp.Views;

public partial class DriverDashboardPage : ContentPage
{
    int driverId;
    SocketIO socket;

    public DriverDashboardPage()
    {
        InitializeComponent();
        driverId = Preferences.Get("userId", 0);

        InitSocket();
    }

    // ✅ SOCKET INIT
    async void InitSocket()
    {
        socket = new SocketIO(new Uri("http://10.0.2.2:3001"));
        await socket.ConnectAsync();
    }

    // ✅ FIX: NOW INSIDE CLASS
    async Task<bool> CheckLocationPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        return status == PermissionStatus.Granted;
    }

    private async void CreateTrip_Clicked(object sender, EventArgs e)
    {
        var data = new
        {
            driver_id = driverId,
            start = startEntry.Text,
            end = endEntry.Text,
            price = priceEntry.Text,
            seats = seatsEntry.Text
        };

        await new HttpClient().PostAsJsonAsync(
            "http://10.0.2.2:3001/create-trip", data);

        await DisplayAlert("Success", "Trip Created", "OK");
    }

    private async void StartTracking_Clicked(object sender, EventArgs e)
    {
        bool granted = await CheckLocationPermission();

        if (!granted)
        {
            await DisplayAlert("Permission", "Location permission denied", "OK");
            return;
        }

        while (true)
        {
            var location = await Geolocation.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.High));

            if (location != null)
            {
                await socket.EmitAsync("send-location", new object[]
 {
    new
    {
        lat = location.Latitude,
        lng = location.Longitude
    }
 });
            }

            await Task.Delay(2000);
        }
    }
}