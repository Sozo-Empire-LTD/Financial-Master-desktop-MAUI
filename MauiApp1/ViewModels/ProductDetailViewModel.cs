using CommunityToolkit.Mvvm.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;



namespace MauiApp1.ViewModels
{
    public partial class ProductDetailViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient; // Declare the _httpClient field

      
        [ObservableProperty]
        private Product _productD;

        [ObservableProperty]
        private Unit _productUnit;

        [ObservableProperty]
        private int _quantityAv;

        [ObservableProperty]
        private string _CategoryName;

        [ObservableProperty]
        private int _purchases;

        [ObservableProperty]
        private int _sale;

        [ObservableProperty]
        private int _pcsUnit;

        [ObservableProperty]
        private int _kgUnit;

        [ObservableProperty]
        private int _boxUnit;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private Search _search;

        [ObservableProperty]
        private string _subTitle;

        [ObservableProperty]
        private Transactions _transactions;

        [ObservableProperty]
        private List<object> _movements;

        public ProductDetailViewModel()
        {
            _httpClient = new HttpClient(); // Initialize the _httpClient field
        }



        public async Task LoadProductDetails(string productSlug)
        {
            try
            {
               

                string apiUrl = $"http://127.0.0.1:8000/api/products/product-Detail/{productSlug}";

                using (HttpClient client = new HttpClient())
                {


                string token = Preferences.Get("AuthToken", string.Empty);
                Debug.WriteLine($"The token is: {token}");

                if (string.IsNullOrEmpty(token))
                {
                    //await DisplayAlert("Error", "Authentication token not found. Please log in again.", "OK");
                    return;
                }

                //string requestUrl = $"{apiUrl}{queryString}";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiUrl);
              

                if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                   
                    string contentType = response.Content.Headers.ContentType?.MediaType;
                    var responseData = JsonSerializer.Deserialize<ProductResponse>(json);

                        if (responseData != null)
                        {
                            

                            ProductD = new Product
                            {
                                Id = responseData.Product.Id,
                                Name = responseData.Product.Name,
                                Price = responseData.Product.Price,
                                CostPrice = responseData.Product.cost_price,
                                CategoryId = responseData.Product.CategoryId,
                                Quantity = responseData.Product.Quantity as int?, // Assuming Quantity is nullable
                                Description = responseData.Product.Description,
                                slug = responseData.Product.Slug,
                                RequiresStock = responseData.Product.RequiresStock,
                                Image = responseData.Product.Image?.ToString(), // Assuming Image is an object
                                WholeSaleQty = responseData.Product.WholeSaleQty as int?, // Assuming WholeSaleQty is nullable
                                WholeSalePrice = responseData.Product.WholeSalePrice as decimal?, // Assuming WholeSalePrice is nullable
                                Location = responseData.Product.Location?.ToString(), // Assuming Location is an object
                                ShopId = responseData.Product.ShopId,
                                LoggedBy = responseData.Product.LoggedBy,
                                CreatedAt = responseData.Product.CreatedAt,
                                UpdatedAt = responseData.Product.UpdatedAt,
                                Min = responseData.Product.Min,
                                Signal = responseData.Product.Signal?.ToString(), // Assuming Signal is an object
                                Code = responseData.Product.Code?.ToString(), // Assuming Code is an object
                                Max = responseData.Product.Max,
                                DeletedAt = responseData.Product.DeletedAt as DateTime?, // Assuming DeletedAt is nullable
                                Total = 0, // Assuming Total is not present in the original ProductResponse.Product
                                Category = null // Assuming Category is not present in the original ProductResponse.Product
                            };


                            ProductUnit = new Unit
                            {
                               BaseUnit = responseData.Product.Unit.BaseUnit,
                            };

                            QuantityAv = responseData.QuantityAv;
                            CategoryName = responseData.CategoryName;
                            Purchases = responseData.Purchases;
                            Sale = responseData.Sale;
                            PcsUnit = responseData.PcsUnit;
                            KgUnit = responseData.KgUnit;
                            BoxUnit = responseData.BoxUnit;
                            Title = responseData.Title;
                            Search = responseData.Search;
                            SubTitle = responseData.SubTitle; 
                            Transactions = responseData.Transactions;
                            Movements = responseData.Movements;
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Failed to load product details.", "OK");
                    }
                }
        }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "the is an error", "OK");
    }
}



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}

public class ProductResponse
{
    [JsonPropertyName("product")]
    public Product Product { get; set; }

    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; }
     
    [JsonPropertyName("quantityav")]
    public int QuantityAv { get; set; }

    [JsonPropertyName("purchases")]
    public int Purchases { get; set; }

    [JsonPropertyName("sale")]
    public int Sale { get; set; }

    [JsonPropertyName("pcs_unit")]
    public int PcsUnit { get; set; }

    [JsonPropertyName("kg_unit")]
    public int KgUnit { get; set; }

    [JsonPropertyName("box_unit")]
    public int BoxUnit { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("search")]
    public Search Search { get; set; }

    [JsonPropertyName("sub_title")]
    public string SubTitle { get; set; }

    [JsonPropertyName("transactions")]
    public Transactions Transactions { get; set; }

    [JsonPropertyName("movements")]
    public List<object> Movements { get; set; } // Assuming movements is an array of objects
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

public class Search
{
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("from")]
    public DateTime From { get; set; }

    [JsonPropertyName("to")]
    public DateTime To { get; set; }
}

public class Transactions
{
    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("data")]
    public List<object> Data { get; set; }

    [JsonPropertyName("first_page_url")]
    public string FirstPageUrl { get; set; }

    [JsonPropertyName("from")]
    public object From { get; set; }

    [JsonPropertyName("last_page")]
    public int LastPage { get; set; }

    [JsonPropertyName("last_page_url")]
    public string LastPageUrl { get; set; }

    [JsonPropertyName("links")]
    public List<Link> Links { get; set; }

    [JsonPropertyName("next_page_url")]
    public object NextPageUrl { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("prev_page_url")]
    public object PrevPageUrl { get; set; }

    [JsonPropertyName("to")]
    public object To { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class Link
{
    [JsonPropertyName("url")]
    public object Url { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }
}



