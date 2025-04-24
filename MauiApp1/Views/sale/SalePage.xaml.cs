using System.Net;
namespace MauiApp1.Views.sale;

public partial class SalePage : ContentPage
{
    private readonly HttpClient client = new HttpClient(); // Add this field to define 'client'

    public SalePage()
    {
        InitializeComponent();
    }

    public async void MoveToSalesTablePage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("SaleTablePage", true);
    }

    public async void MoveToCreateSalesPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("CreateSalePage", true);
    }

    private async void FilterButton_Clicked(object sender, EventArgs e)
    {
        string searchQuery = SearchEntry.Text ?? string.Empty;
        DateTime startDate = StartDate.Date;
        DateTime endDate = EndDate.Date;
        string agentName = string.Empty;

        if (endDate < startDate)
        {
            await DisplayAlert("Validation Error", "End date cannot be earlier than start date.", "OK");
            return;
        }

        string formattedStartDate = startDate.ToString("yyyy-MM-dd");
        string formattedEndDate = endDate.ToString("yyyy-MM-dd");
        string formattedAgentName = WebUtility.UrlEncode(agentName);
        string encodedSearchQuery = WebUtility.UrlEncode(searchQuery);

        string baseApiUrl = "http://127.0.0.1:8000/api/pos/sales/";
        string requestUrl = $"{baseApiUrl}?q={encodedSearchQuery}&from={formattedStartDate}&to={formattedEndDate}&agent_name={formattedAgentName}";

        Console.WriteLine($"Calling API: {requestUrl}");

        try
        {
            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Success Response: " + responseBody);
                await DisplayAlert("Success", "Filter applied successfully!", "OK");
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                await DisplayAlert("API Error", $"Failed to apply filter. Status: {response.StatusCode}\nDetails: {errorContent}", "OK");
            }
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"Network Error: {httpEx.Message}");
            await DisplayAlert("Network Error", $"Could not connect to the server: {httpEx.Message}", "OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }
}
