using ShuttleTrackerApp.Services;

namespace ShuttleTrackerApp.Views;

public partial class LoginPage : ContentPage
{
    ApiService api = new ApiService();

    public LoginPage()
    {
        InitializeComponent();
    }
    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            var response = await api.Post<LoginResponse>("/login", new
            {
                email = emailEntry.Text,
                password = passwordEntry.Text
            });

            if (!string.IsNullOrEmpty(response.error))
            {
                await DisplayAlert("Error", response.error, "OK");
                return;
            }

            // now safe to use
            string role = response.role.ToString();

            if (role == "admin")
            {
                await Navigation.PushAsync(new AdminDashboardPage());
            }
            else if (role == "driver")
            {
                await Navigation.PushAsync(new DriverDashboardPage());
            }
            else
            {
                await Navigation.PushAsync(new StudentDashboardPage());
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
