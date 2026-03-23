using Microsoft.Maui.Storage;
using ShuttleTrackerApp.Services;

namespace ShuttleTrackerApp.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        ApiService api = new ApiService();

        await api.Register(
            nameEntry.Text,
            emailEntry.Text,
            passwordEntry.Text
        );

        await DisplayAlert("Success", "Registered", "OK");
    }
}