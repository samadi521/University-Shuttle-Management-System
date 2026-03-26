using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using ShuttleTrackerApp.Models;
using ShuttleTrackerApp.Services;
using SocketIOClient;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ShuttleTrackerApp.Views;

public partial class StudentDashboardPage : ContentPage
{
    SocketIO? socket;
    ObservableCollection<BusModel> buses = new();

    public StudentDashboardPage()
    {
        InitializeComponent();
        busList.ItemsSource = buses;
    }

    async Task<bool> CheckPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        return status == PermissionStatus.Granted;
    }

    async void LoadStudentLocation()
    {
        if (!await CheckPermission()) return;

        var location = await Geolocation.GetLocationAsync(
            new GeolocationRequest(GeolocationAccuracy.High));

        if (location == null) return;

        var studentLocation = new Location(location.Latitude, location.Longitude);

        map.Pins.Remove(map.Pins.FirstOrDefault(p => p.Label == "Me"));

        map.Pins.Add(new Pin
        {
            Label = "Me",
            Location = studentLocation
        });

        map.MoveToRegion(
            MapSpan.FromCenterAndRadius(studentLocation, Distance.FromKilometers(2)));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadTripsFromApi();
        await InitSocket();

        LoadStudentLocation();
    }

    async Task LoadTripsFromApi()
    {
        var api = new ApiService();
        var tripList = await api.Get<List<dynamic>>("/trips");

        buses.Clear();

        foreach (var t in tripList)
        {
            buses.Add(new BusModel
            {
                BusId = (int)t.id,
                Start = (string)t.start_location,
                End = (string)t.end_location,
                Price = (double)t.price,
                Seats = (int)t.available_seats
            });
        }
    }

    async Task InitSocket()
    {
        socket = SocketService.GetSocket();
        await SocketService.ConnectAsync();

        socket.On("busUpdate", async (response) =>
        {
            var json = response.ToString();
            using var doc = JsonDocument.Parse(json);

            var bus = new BusModel
            {
                BusId = doc.RootElement.GetProperty("busId").GetInt32(),
                Lat = doc.RootElement.GetProperty("lat").GetDouble(),
                Lng = doc.RootElement.GetProperty("lng").GetDouble(),
                Price = doc.RootElement.GetProperty("price").GetDouble(),
                Seats = doc.RootElement.GetProperty("seats").GetInt32(),
                Countdown = doc.RootElement.GetProperty("countdown").GetInt32(),
                Start = doc.RootElement.GetProperty("start").GetString(),
                End = doc.RootElement.GetProperty("end").GetString()
            };
            double startLat = doc.RootElement.GetProperty("startLat").GetDouble();
            double startLng = doc.RootElement.GetProperty("startLng").GetDouble();
            double endLat = doc.RootElement.GetProperty("endLat").GetDouble();
            double endLng = doc.RootElement.GetProperty("endLng").GetDouble();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var existing = buses.FirstOrDefault(b => b.BusId == bus.BusId);

                if (existing == null)
                    buses.Add(bus);
                else
                {
                    existing.Price = bus.Price;
                    existing.Seats = bus.Seats;
                    existing.Countdown = bus.Countdown;
                    existing.Lat = bus.Lat;
                    existing.Lng = bus.Lng;
                }

                UpdateBusOnMap(bus);
            });
        });
    }
    private async void OnDetailsClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var bus = button?.BindingContext as BusModel;

        if (bus != null)
        {
            await DisplayAlert("Tracking",
                $"Bus {bus.BusId}\nFrom: {bus.Start}\nTo: {bus.End}",
                "OK");
        }
    }
    void UpdateBusOnMap(BusModel bus)
    {
        if (bus.Lat == 0 || bus.Lng == 0) return;

        var busLocation = new Location(bus.Lat, bus.Lng);

        map.Pins.Remove(map.Pins.FirstOrDefault(p => p.Label == $"Bus {bus.BusId}"));

        map.Pins.Add(new Pin
        {
            Label = $"Bus {bus.BusId}",
            Location = busLocation
        });
    }
    private async void OnBookingClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var bus = button?.BindingContext as BusModel;

        if (bus != null)
        {
            await Navigation.PushAsync(new BookingPage(bus));
        }
    }
    void DrawRoute(double startLat, double startLng, double endLat, double endLng)
    {
        map.MapElements.Clear();

        var polyline = new Polyline
        {
            StrokeColor = Colors.Blue,
            StrokeWidth = 5
        };

        polyline.Geopath.Add(new Location(startLat, startLng));
        polyline.Geopath.Add(new Location(endLat, endLng));

        map.MapElements.Add(polyline);
    }
}