using System.Net.Http.Json;

namespace ShuttleTrackerApp.Views;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage()
    {
        InitializeComponent();
    }

    private async void AddDriver_Clicked(object sender, EventArgs e)
    {
        var data = new
        {
            name = nameEntry.Text,
            email = emailEntry.Text,
            password = passwordEntry.Text
        };

        await new HttpClient().PostAsJsonAsync(
            "http://10.0.2.2:3001/add-driver", data);

        await DisplayAlert("Success", "Driver Added", "OK");
    }
}