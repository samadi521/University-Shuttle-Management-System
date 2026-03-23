using Microsoft.Maui.Maps;
using System.Text.Json;

namespace ShuttleTrackerApp.Views;

public partial class AdminMapPage : ContentPage
{
    public AdminMapPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        int driverId = 1; // later dynamic

        while (true)
        {
            var json = await new HttpClient()
                .GetStringAsync($"http://10.0.2.2:3001/driver-location/{driverId}");

            var loc = JsonDocument.Parse(json);

            double lat = loc.RootElement.GetProperty("latitude").GetDouble();
            double lng = loc.RootElement.GetProperty("longitude").GetDouble();

            map.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Location(lat, lng),
                Distance.FromKilometers(1)));

            await Task.Delay(5000);
        }
    }
}