using ShuttleTrackerApp.Services;

namespace ShuttleTrackerApp.Views;

public partial class BookingPage : ContentPage
{
    BusModel bus;

    public BookingPage(BusModel selectedBus)
    {
        InitializeComponent();
        bus = selectedBus;

        startLabel.Text = bus.Start;
        endLabel.Text = bus.End;
        priceLabel.Text = $"Price: {bus.Price}";
        seatsLabel.Text = $"Seats: {bus.Seats}";
    }
  
private async void OnBookClicked(object sender, EventArgs e)
    {
        try
        {
            var api = new ApiService();

            await api.Post<object>("/book-trip", new
            {
                user_id = 1,
                trip_id = bus.BusId
            });

            await DisplayAlert("Success", "Booking Done!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}

