using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using MauiApp1.Models;




namespace MauiApp1.Views.products;

public partial class CreateProductPage : ContentPage
{
	
    private readonly HttpClient _httpClient;

    public ObservableCollection<Category> Categories { get; set; }
    private Category _selectedCategoryObject;

    private void CategoryPicker_SelectedItemChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker && picker.SelectedItem is Category selectedCategory)
        {
            _selectedCategoryObject = selectedCategory;
        }
    }

    public ObservableCollection<SignalOption> SignalOptions { get; set; }

    private SignalOption _selectedSignalOption;

    public CreateProductPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        Categories = new ObservableCollection<Category>();
        Task.Run(async () => await LoadCategoriesAsync());
        saveButton.Clicked += OnSaveClicked;

        SignalOptions = new ObservableCollection<SignalOption>
        {
            new SignalOption { Display = "21 days", Value = 21 },
            new SignalOption { Display = "10 days", Value = 10 },
            new SignalOption { Display = "5 days", Value = 5 },
            new SignalOption { Display = "3 days", Value = 3 },
            new SignalOption { Display = "1 day", Value = 1 }
        };
        BindingContext = this; // Explicitly set the BindingContext

        OnPropertyChanged(nameof(SignalOptions));
    }


    private void SignalPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker && picker.SelectedItem is SignalOption selectedSignal)
        {
            _selectedSignalOption = selectedSignal;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            saveButton.IsEnabled = false; // Disable the save button immediately

            // 1. Get data from the fields
            var product = new Product
            {
                Name = name.Text,
                Description = description.Text,
                Category = _selectedCategoryObject?.Id ?? 0, // Use selected category ID
                Price = double.TryParse(price.Text, out double parsedPrice) ? parsedPrice : 0,
                cost_price = double.TryParse(cost_price.Text, out double parsedCostPrice) ? parsedCostPrice : 0,
                base_unit = base_unit.SelectedItem?.ToString(), // Check for null
                pcs_unit = int.TryParse(pcs_unit.Text, out int parsedPcsUnit) ? parsedPcsUnit : 0,
                kg_unit = int.TryParse(kg_unit.Text, out int parsedKgUnit) ? parsedKgUnit : 0,
                box_unit = int.TryParse(box_unit.Text, out int parsedBoxUnit) ? parsedBoxUnit : 0,
                Location = location.SelectedItem?.ToString(), // Check for null
                Min = int.TryParse(min.Text, out int parsedMin) ? parsedMin : 0,
                Max = int.TryParse(max.Text, out int parsedMax) ? parsedMax : 0,
                Code = code.Text,
                signal = _selectedSignalOption?.Value.ToString() ?? "0" // Get the numeric value
            };

            // Format the product details into a string
            string productDetails = $"Name: {product.Name}\n" +
                                    $"Description: {product.Description}\n" +
                                    $"Category ID: {product.Category}\n" +
                                    $"Price: {product.Price}\n" +
                                    $"Cost Price: {product.cost_price}\n" +
                                    $"Base Unit: {product.base_unit}\n" +
                                    $"Pcs Unit: {product.pcs_unit}\n" +
                                    $"Kg Unit: {product.kg_unit}\n" +
                                    $"Box Unit: {product.box_unit}\n" +
                                    $"Location: {product.Location}\n" +
                                    $"Min: {product.Min}\n" +
                                    $"Max: {product.Max}\n" +
                                    $"Code: {product.Code}\n" +
                                    $"Signal: {product.signal}";

            await DisplayAlert("Product Details", productDetails, "OK");

            // 2. Save data to the database using the API endpoint
            int businessId = Preferences.Get("businessId", -1);
            string token = Preferences.Get("AuthToken", string.Empty);
            // Configure the Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string apiUrl = $"http://127.0.0.1:8000/api/pos/products/create/{businessId}";
            var response = await _httpClient.PostAsJsonAsync(apiUrl, product);

            if (response.IsSuccessStatusCode)
            {
                // Handle successful save
                await DisplayAlert("Success", "Product saved successfully.", "OK");
                await Navigation.PopAsync(); // Navigate back or clear fields
            }
            else
            {
                // Handle API error
                string errorMessage = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"Failed to save product. Error: {errorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., network issues)
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
        finally
        {
            saveButton.IsEnabled = true; // Re-enable the button in all cases
        }
    }

  

    public async Task LoadCategoriesAsync()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                int businessId = Preferences.Get("businessId", -1);
                string token = Preferences.Get("AuthToken", string.Empty);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string apiUrl = $"http://127.0.0.1:8000/api/products/categories/{businessId}";

                var categoryResponse = await client.GetFromJsonAsync<CategoryResponse>(apiUrl);

                MainThread.BeginInvokeOnMainThread(() =>
                {

                    if (categoryResponse?.categories != null)
                    {
                       
                        Categories.Clear();
                        foreach (var category in categoryResponse.categories)
                        {
                            
                            Categories.Add(new Category
                            {
                                Id = category.Id,
                                Name = category.Name
                            });
                        }
                    }
                    OnPropertyChanged(nameof(Categories));
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load categories: {ex.Message}", "OK");
        }
    }

    private async void OnProductCreateCancel(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///ProductsPage");
        //await Navigation.PopAsync();

    }



    // Define your Product class to match the API's expected data structure.
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Category { get; set; }
        public double Price { get; set; }
        public double cost_price { get; set; }
        public string base_unit { get; set; }
        public int pcs_unit { get; set; }
        public int kg_unit { get; set; }
        public int box_unit { get; set; }
        public string Location { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string Code { get; set; }
        public string signal { get; set; }

        // Add other properties as needed
    }

    public class SignalOption
    {
        public string Display { get; set; }
        public int Value { get; set; }
    }



    public class CategoryResponse
    {
        public List<Category> categories { get; set; }
    }
}