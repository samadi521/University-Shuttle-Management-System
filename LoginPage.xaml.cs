using System.Text.Json;
using ShuttleTrackerApp.Services;

namespace ShuttleTrackerApp.Views;

public partial class LoginPage : ContentPage
{
    ApiService api = new ApiService();

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var response = await api.Login(emailEntry.Text, passwordEntry.Text);

        if (response.Contains("error"))
        {
            await DisplayAlert("Error", "Invalid login", "OK");
            return;
        }

        var json = JsonDocument.Parse(response);

        string role = json.RootElement.GetProperty("role").GetString();
        int userId = json.RootElement.GetProperty("userId").GetInt32();

        Preferences.Set("userId", userId);
        Preferences.Set("role", role);

        if (role == "admin")
            await Navigation.PushAsync(new AdminDashboardPage());

        else if (role == "driver")
            await Navigation.PushAsync(new DriverDashboardPage());

        else
            await Navigation.PushAsync(new StudentDashboardPage());
    }
}