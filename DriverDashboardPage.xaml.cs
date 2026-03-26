using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using SocketIOClient;
using ShuttleTrackerApp.Services;
using System.Text.Json;
using ShuttleTrackerApp.Models;

namespace ShuttleTrackerApp.Views;

public partial class DriverDashboardPage : ContentPage
{
    SocketIO? socket;
    CancellationTokenSource? cts;

    int busId = 1;
    int countdown = 0;
    bool isCounting = false;
    bool tripStarted = false;

    public DriverDashboardPage()
    {
        InitializeComponent();
        Init();
    }

    async void Init()
    {
        socket = SocketService.GetSocket();
        await SocketService.ConnectAsync();

        await CheckAndRequestPermission();
    }

    //  AUTO GPS TRACKING + ROUTE SEND
    async void StartAutoTracking()
    {
        while (true)
        {
            try
            {
                var location = await Geolocation.GetLocationAsync(
                    new GeolocationRequest(GeolocationAccuracy.High));

                if (location != null && socket != null && tripStarted)
                {
                    //  LOAD ROUTE FROM STORAGE
                    var routeJson = Preferences.Get("bus_route", null);

                    double startLat = 0, startLng = 0, endLat = 0, endLng = 0;

                    if (!string.IsNullOrEmpty(routeJson))
                    {
                        var route = JsonSerializer.Deserialize<RouteModel>(routeJson);

                        if (route != null)
                        {
                            startLat = route.startLat;
                            startLng = route.startLng;
                            endLat = route.endLat;
                            endLng = route.endLng;
                        }
                    }

                    // SEND FULL DATA TO SERVER
                    await socket.EmitAsync("sendLocation", new object[]
                    {
                        new
                        {
                            busId = busId,
                            lat = location.Latitude,
                            lng = location.Longitude,

                            start = startEntry.Text,
                            end = endEntry.Text,

                            startLat,
                            startLng,
                            endLat,
                            endLng,

                            price = int.Parse(priceEntry.Text),
                            seats = int.Parse(seatsEntry.Text),
                            countdown = countdown
                        }
                    });
                }
            }
            catch { }

            await Task.Delay(3000);
        }
    }

    // PERMISSION
    async Task<bool> CheckAndRequestPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Permission Required",
                "Location permission is needed to track bus location", "OK");
            return false;
        }

        return true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartAutoTracking();
    }

    //  CREATE TRIP + SAVE ROUTE TO BACKEND
    private async void OnStartTripClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(startEntry.Text) ||
            string.IsNullOrWhiteSpace(endEntry.Text) ||
            string.IsNullOrWhiteSpace(priceEntry.Text) ||
            string.IsNullOrWhiteSpace(seatsEntry.Text))
        {
            await DisplayAlert("Error", "Fill all fields", "OK");
            return;
        }

        try
        {
            var routeJson = Preferences.Get("bus_route", null);

            double startLat = 0, startLng = 0, endLat = 0, endLng = 0;

            if (!string.IsNullOrEmpty(routeJson))
            {
                var route = JsonSerializer.Deserialize<RouteModel>(routeJson);

                if (route != null)
                {
                    startLat = route.startLat;
                    startLng = route.startLng;
                    endLat = route.endLat;
                    endLng = route.endLng;
                }
            }

            var api = new ApiService();

            var result = await api.Post<dynamic>("/create-trip", new
            {
                driver_id = 1,
                start = startEntry.Text,
                end = endEntry.Text,
                price = int.Parse(priceEntry.Text),
                seats = int.Parse(seatsEntry.Text),

                startLat,
                startLng,
                endLat,
                endLng
            });

            busId = (int)result.tripId;
            tripStarted = true;

            await DisplayAlert("Success", "Trip + Route saved!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cts?.Cancel();
    }

    //  COUNTDOWN
    private void OnStartCountdownClicked(object sender, EventArgs e)
    {
        countdown = 300;
        isCounting = true;

        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            if (!isCounting) return false;

            if (countdown > 0)
            {
                countdown--;
                countdownLabel.Text = $"⏳ Countdown: {countdown}s";
                return true;
            }

            isCounting = false;
            return false;
        });
    }

    // OPEN MAP PAGE
    async void OnOpenMapClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DriverMapPage());
    }
}