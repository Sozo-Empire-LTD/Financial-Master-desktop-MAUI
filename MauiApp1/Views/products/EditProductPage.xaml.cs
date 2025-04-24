using System.Collections.ObjectModel;

using System.Diagnostics;

using System.Net.Http.Headers;

using System.Net.Http.Json;

using System.Text.Json;

using System.Text.Json.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;



using MauiApp1.Models;



namespace MauiApp1.Views.products;

public interface ObservableObject;

[QueryProperty(nameof(ProductSlug), "productSlug")]

public partial class EditProductPage : ContentPage, ObservableObject

{

    private readonly HttpClient _httpClient;


    public string ProductSlug
    {
        get => _productSlug;
        set
        {
            _productSlug = value;
            Debug.WriteLine($"ProductSlug received in EditProductPage: {_productSlug}");
            // Now that we have the slug, load the product details
            Task.Run(LoadProductDetails);
        }
    }
    private string _productSlug;



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



    public EditProductPage()

    {

        InitializeComponent();

        _httpClient = new HttpClient();

        Debug.WriteLine($"the product slug in edit product page is: {ProductSlug}");

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



    public async Task LoadProductDetails()

    {

        try

        {

            Debug.WriteLine($"the product slug: {ProductSlug}");



            string apiUrl = $"http://127.0.0.1:8000/api/products/product-Detail/{ProductSlug}";



            using (HttpClient client = new HttpClient())

            {

                string token = Preferences.Get("AuthToken", string.Empty);

                Debug.WriteLine($"The token is: {token}");



                if (string.IsNullOrEmpty(token))

                {

                    await DisplayAlert("Error", "Authentication token not found. Please log in again.", "OK");

                    return;

                }



                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));



                HttpResponseMessage response = await client.GetAsync(apiUrl);



                if (response.IsSuccessStatusCode)

                {

                    string json = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine($"API Response: {json}"); // Log the raw JSON

                    var responseData = JsonSerializer.Deserialize<ProductResponse>(json);



                    if (responseData?.Product != null)

                    {

                        MainThread.BeginInvokeOnMainThread(() =>

                        {

                            name.Text = responseData.Product.Name;

                            description.Text = responseData.Product.Description;



                            if (responseData.Product.CategoryId != 0 && Categories.Any(c => c.Id == responseData.Product.CategoryId))

                            {

                                categoryPicker.SelectedItem = Categories.First(c => c.Id == responseData.Product.CategoryId);

                                _selectedCategoryObject = (Category)categoryPicker.SelectedItem;

                            }



                            price.Text = responseData.Product.Price.ToString();

                            cost_price.Text = responseData.Product.cost_price.ToString();

                            base_unit.SelectedItem = responseData.Product.Unit?.BaseUnit; 

                            pcs_unit.Text = responseData.Product.Unit?.PcsUnit;

                            kg_unit.Text = responseData.Product.Unit?.KgUnit;   

                            box_unit.Text = responseData.Product.Unit?.BoxUnit; 

                            location.SelectedItem = responseData.Product.Location?.ToString();



                            min.Text = responseData.Product.Min.ToString();

                            max.Text = responseData.Product.Max.ToString();

                            code.Text = responseData.Product.Code?.ToString();



                            if (!string.IsNullOrEmpty(responseData.Product.Signal?.ToString()) && int.TryParse(responseData.Product.Signal.ToString(), out int signalValue) && SignalOptions.Any(s => s.Value == signalValue))

                            {

                                signal.SelectedItem = SignalOptions.First(s => s.Value == signalValue);

                                _selectedSignalOption = (SignalOption)signal.SelectedItem;

                            }

                        });

                    }

                    else

                    {

                        await DisplayAlert("Error", "Failed to load product details.", "OK");

                        await Navigation.PopAsync();

                    }

                }

                else

                {

                    await DisplayAlert("Error", $"Failed to load product details. Status Code: {response.StatusCode}", "OK");

                }

            }

        }

        catch (Exception ex)

        {

            await DisplayAlert("Error", $"An error occurred while loading product: {ex.Message}", "OK");

        }

    }



    private async void OnSaveClicked(object sender, EventArgs e)

    {

        try

        {

            saveButton.IsEnabled = false; // Disable the save button immediately



            // 1. Get data from the fields

            var productToUpdate = new ProductUpdateRequest // Use a specific class for updates

            {

                Name = name.Text,

                Description = description.Text,

                CategoryId = _selectedCategoryObject?.Id ?? 0, // Use selected category ID

                Price = double.TryParse(price.Text, out double parsedPrice) ? parsedPrice : 0,

                CostPrice = double.TryParse(cost_price.Text, out double parsedCostPrice) ? parsedCostPrice : 0,

                BaseUnit = base_unit.SelectedItem?.ToString(), // Check for null

                PcsUnit = int.TryParse(pcs_unit.Text, out int parsedPcsUnit) ? parsedPcsUnit : 1,

                KgUnit = int.TryParse(kg_unit.Text, out int parsedKgUnit) ? parsedKgUnit : 1,

                BoxUnit = int.TryParse(box_unit.Text, out int parsedBoxUnit) ? parsedBoxUnit : 1,

                Location = location.SelectedItem?.ToString(), // Check for null

                Min = int.TryParse(min.Text, out int parsedMin) ? parsedMin : 0,

                Max = int.TryParse(max.Text, out int parsedMax) ? parsedMax : 0,

                Code = code.Text,

                Signal = _selectedSignalOption?.Value.ToString() // Get the numeric value as string

            };



            int businessId = Preferences.Get("businessId", -1);

            string token = Preferences.Get("AuthToken", string.Empty);

            // Configure the Authorization header

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string apiUrl = $"http://127.0.0.1:8000/api/pos/products/update/{ProductSlug}/{businessId}"; // Use PUT for updates

            var response = await _httpClient.PutAsJsonAsync(apiUrl, productToUpdate);



            if (response.IsSuccessStatusCode)

            {

                // Handle successful update

                await DisplayAlert("Success", "Product updated successfully.", "OK");

                await Shell.Current.GoToAsync("///ProductsPage");

            }

            else

            {

                // Handle API error

                string errorMessage = await response.Content.ReadAsStringAsync();

                await DisplayAlert("Error", $"Failed to update product. Error: {errorMessage}", "OK");

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



    private async void OnProductEditCancel(object sender, EventArgs e)

    {

        await Shell.Current.GoToAsync("///ProductsPage");

    }



    // Define a class specifically for the update request

    public class ProductUpdateRequest

    {

        [JsonPropertyName("name")]

        public string Name { get; set; }

        [JsonPropertyName("description")]

        public string Description { get; set; }

        [JsonPropertyName("category_id")]

        public int CategoryId { get; set; }

        [JsonPropertyName("price")]

        public double Price { get; set; }

        [JsonPropertyName("cost_price")]

        public double CostPrice { get; set; }

        [JsonPropertyName("base_unit")]

        public string BaseUnit { get; set; }

        [JsonPropertyName("pcs_unit")]

        public int PcsUnit { get; set; }

        [JsonPropertyName("kg_unit")]

        public int KgUnit { get; set; }

        [JsonPropertyName("box_unit")]

        public int BoxUnit { get; set; }

        [JsonPropertyName("location")]

        public string Location { get; set; }

        [JsonPropertyName("min")]

        public int Min { get; set; }

        [JsonPropertyName("max")]

        public int Max { get; set; }

        [JsonPropertyName("code")]

        public string Code { get; set; }

        [JsonPropertyName("signal")]

        public string Signal { get; set; }

        // Add other properties as needed for the update

    }



    public class SignalOption

    {

        public string Display { get; set; }

        public int Value { get; set; }

    }



    public class CategoryResponse

    {

        [JsonPropertyName("categories")]

        public List<Category> categories { get; set; }

    }



    public class ProductResponse

    {

        [JsonPropertyName("product")]

        public Product Product { get; set; }



        // ... other properties from the response, ensure Product class below matches the 'product' node

    }



    public class Product

    {

        [JsonPropertyName("id")]

        public int Id { get; set; }



        [JsonPropertyName("name")]

        public string Name { get; set; }



        [JsonPropertyName("price")]

        public decimal Price { get; set; }



        [JsonPropertyName("cost_price")]

        public decimal cost_price { get; set; }



        [JsonPropertyName("category_id")]

        public int CategoryId { get; set; }



        [JsonPropertyName("quantity")]

        public object Quantity { get; set; }



        [JsonPropertyName("description")]

        public string Description { get; set; }



        [JsonPropertyName("slug")]

        public string Slug { get; set; }



        [JsonPropertyName("requires_stock")]

        public int RequiresStock { get; set; }



        [JsonPropertyName("image")]

        public object Image { get; set; }



        [JsonPropertyName("whole_sale_qty")]

        public object WholeSaleQty { get; set; }



        [JsonPropertyName("whole_sale_price")]

        public object WholeSalePrice { get; set; }



        [JsonPropertyName("location")]

        public object Location { get; set; }



        [JsonPropertyName("shop_id")]

        public int ShopId { get; set; }



        [JsonPropertyName("logged_by")]

        public int LoggedBy { get; set; }



        [JsonPropertyName("created_at")]

        public DateTime CreatedAt { get; set; }



        [JsonPropertyName("updated_at")]

        public DateTime UpdatedAt { get; set; }



        [JsonPropertyName("min")]

        public int Min { get; set; }



        [JsonPropertyName("signal")]

        public object Signal { get; set; }



        [JsonPropertyName("code")]

        public object Code { get; set; }



        [JsonPropertyName("max")]

        public int Max { get; set; }



        [JsonPropertyName("deleted_at")]

        public object DeletedAt { get; set; }



        [JsonPropertyName("unit")]

        public Unit Unit { get; set; }

    }



    public class Unit

    {

        [JsonPropertyName("id")]

        public int Id { get; set; }



        [JsonPropertyName("product_id")]

        public int ProductId { get; set; }



        [JsonPropertyName("base_unit")]

        public string BaseUnit { get; set; }



        [JsonPropertyName("kg_unit")]

        public string KgUnit { get; set; }



        [JsonPropertyName("box_unit")]

        public string BoxUnit { get; set; }



        [JsonPropertyName("pcs_unit")]

        public string PcsUnit { get; set; }



        [JsonPropertyName("created_at")]

        public DateTime CreatedAt { get; set; }



        [JsonPropertyName("updated_at")]

        public DateTime UpdatedAt { get; set; }

    }

}