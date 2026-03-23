using System.Net.Http.Json;

namespace ShuttleTrackerApp.Services;

public class ApiService
{
    HttpClient client = new HttpClient();
    string baseUrl = "http://10.0.2.2:3001";

    public async Task<string> Login(string email, string password)
    {
        var res = await client.PostAsJsonAsync($"{baseUrl}/login", new
        {
            email,
            password
        });

        return await res.Content.ReadAsStringAsync();
    }

    // ✅ FIX ERROR HERE
    public async Task<string> Register(string name, string email, string password)
    {
        var res = await client.PostAsJsonAsync($"{baseUrl}/register", new
        {
            name,
            email,
            password
        });

        return await res.Content.ReadAsStringAsync();
    }

    public async Task<string> GetTrips()
    {
        return await client.GetStringAsync($"{baseUrl}/trips");
    }
}