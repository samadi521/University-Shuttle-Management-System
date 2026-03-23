namespace ShuttleTrackerApp.Views;

public partial class StudentDashboardPage : ContentPage
{
    public StudentDashboardPage()
    {
        InitializeComponent();
    }

    private async void ViewTrips_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BookingPage());
    }
}