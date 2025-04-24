using System.Collections.ObjectModel;
using System.Threading.Tasks;

using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Web;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;









namespace MauiApp1.ViewModels;

public partial class ProductViewModel : ObservableObject
{
   
    //private const string ApiUrl = "http://127.0.0.1:8000/api/get-products/";
    private readonly HttpClient _httpClient;


    //private readonly ProductApiService _productApiService;
    public ICommand PreviousPageCommand => new Command(async () => await PreviousPage());
    public ICommand NextPageCommand => new Command(async () => await NextPage());

    private string _searchText;
    public string SearchText
    {
        get { return _searchText; }
        set { _searchText = value; OnPropertyChanged();}
    }

    private Category _selectedCategory;
    public Category SelectedCategory
    {
        get { return _selectedCategory; }
        set { _selectedCategory = value; OnPropertyChanged();}
    }


    public ICommand LoadProductsCommand { get; set; }
    public ICommand ProductTappedCommand { get; set; }

    public ObservableCollection<Product> Products { get; set; }
    public ObservableCollection<Category> LocationCategories { get; set; }
    public ObservableCollection<Category> Categories { get; set; }
    public Filter Filters { get; set; }
    public Pagination Pagination { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ProductViewModel()
    {

        _httpClient = new HttpClient();
        Products = new ObservableCollection<Product>();
        LocationCategories = new ObservableCollection<Category>();
        Categories = new ObservableCollection<Category>();
        Filters = new Filter();
        Pagination = new Pagination();

        // Initialize the ProductTappedCommand here
        ProductTappedCommand = new Command<Product>(async (product) => await OnProductTapped(product));

    }


    public async Task LoadProducts()
    {
        try
        {
            

            int businessId = Preferences.Get("businessId", -1);
            Debug.WriteLine($"The businessId is: {businessId}");

            //if(Filters != null) { 

            //  queryString = $"?q={Filters.SearchQuery}&category_id={Filters.SelectedCategory.Id}&sort={Filters.SortBy}&per_page={Pagination.PerPage}&page={Pagination.CurrentPage}";
            // }
            //string apiUrl = $"http://127.0.0.1:8000/api/get-products/{businessId}";

            //if (!string.IsNullOrWhiteSpace(SearchText))
            //{
            //    apiUrl += $"searchText={HttpUtility.UrlEncode(SearchText)}&";
            //}

            //if (SelectedCategory != null)
            //{
            //    apiUrl += $"categoryId={SelectedCategory.Id}&"; // Pass the Category Id
            //}

            //apiUrl = apiUrl.TrimEnd('&');

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                queryParams["searchText"] = SearchText;
            }
            if (SelectedCategory != null)
            {
                queryParams["categoryId"] = SelectedCategory.Id.ToString();
            }

            string apiUrl = $"http://127.0.0.1:8000/api/get-products/{businessId}?{queryParams}";

            string token = Preferences.Get("AuthToken", string.Empty);
            Debug.WriteLine($"The token is: {token}");

            if (string.IsNullOrEmpty(token))
            {
                //await DisplayAlert("Error", "Authentication token not found. Please log in again.", "OK");
                return;
            }

            //string requestUrl = $"{apiUrl}{queryString}";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var response = await _httpClient.GetAsync<ApiResponse>(ApiUrl + queryString);
            var  response = await _httpClient.GetFromJsonAsync<ApiResponse>(apiUrl);


            Debug.WriteLine($"response: {response}");
            var apiResponse = response;

                if (apiResponse?.Data != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Debug.WriteLine($"The token is: {apiResponse.Data}");
                        Products.Clear();

                        if (apiResponse.Data.Products != null)
                        {

                           
                            try
                            {
                                foreach (var product in apiResponse.Data.Products)
                                {
                                    //Debug.WriteLine($"product {product.Id}  is: {product.slug}");
                                    Products.Add(new Product
                                    {
                                        Id = product.Id,
                                        Name = product.Name,
                                        Price = product.Price,
                                        slug = product.slug,
                                        Image = string.IsNullOrEmpty(product.Image) ? "noimage.png" : product.Image,

                                    });

                                }


                                Debug.WriteLine($"the total number of products is: {Products.Count()}");

                            }
                            catch (Exception ex)
                            {

                                Debug.WriteLine($"Error adding product: {ex.Message}");
                            }
                           

                            Debug.WriteLine($"the total number of populated products is: {Products}");
                        }

                        OnPropertyChanged(nameof(Products));
                    });

                    LocationCategories.Clear();
                    if (apiResponse.Data.LocationCategories != null)
                    {
                        foreach (var locationCategory in apiResponse.Data.LocationCategories)
                        {
                            LocationCategories.Add(new Category
                            {
                                Id = locationCategory.Id,
                                Name = locationCategory.Name
                            });
                        }
                    }

                    Categories.Clear();
                    if (apiResponse.Data.Categories != null)
                    {
                        foreach (var category in apiResponse.Data.Categories)
                        {
                            Categories.Add(new Category
                            {
                                Id = category.Id,
                                Name = category.Name
                            });
                        }
                    }

                    if (apiResponse.Data.Pagination != null)
                    {
                        Pagination.Total = apiResponse.Data.Pagination.Total;
                        Pagination.PerPage = apiResponse.Data.Pagination.PerPage;
                        Pagination.CurrentPage = apiResponse.Data.Pagination.CurrentPage;
                        Pagination.LastPage = apiResponse.Data.Pagination.LastPage;
                        Pagination.From = apiResponse.Data.Pagination.From;
                        Pagination.To = apiResponse.Data.Pagination.To;
                    }
                }
           
            

        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., show an error message)
            Console.WriteLine($"Error loading products: {ex.Message}");
        }
    }

    // Example methods for handling filter changes or pagination
    public async Task ApplyFilters()
    {
        Pagination.CurrentPage = 1; // Reset to first page
        await LoadProducts();
    }

 
    public async Task OnProductTapped(Product product)
    {
        if (product == null) return;
       

        // Navigate to the ProductDetailsPage, passing the product ID as a parameter
        await Shell.Current.GoToAsync($"ViewProductPage?productSlug={product.slug}");
    }


    /// <summary>
    /// Navigates to ProductDetailPage with product ID
    /// </summary>
    //[RelayCommand]
    //public async Task ViewProductDetailAsync(Product selectedProduct)
    //{
    //    if (selectedProduct == null) return;

    //    await Shell.Current.GoToAsync($"ViewProductPage?productId={selectedProduct.Id}");
    //}

    public async Task ChangePage(int page)
    {
        Pagination.CurrentPage = page;
        await LoadProducts();
    }

    public async Task PreviousPage()
    {
        if (Pagination.CurrentPage > 1)
        {
            Pagination.CurrentPage--;
            await LoadProducts();
        }
    }

    public async Task NextPage()
    {
        if (Pagination.CurrentPage < Pagination.LastPage)
        {
            Pagination.CurrentPage++;
            await LoadProducts();
        }
    }

   

}

// Model classes for API response


public class Pagination
{
    public int Total { get; set; }
    public int PerPage { get; set; }
    public int CurrentPage { get; set; }
    public int LastPage { get; set; }
    public int From { get; set; }
    public int To { get; set; }
}


    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public int CategoryId { get; set; }
        public int? Quantity { get; set; }
        public string Description { get; set; }
        public string slug { get; set; }
        public int RequiresStock { get; set; }
        public string Image { get; set; }
        public int? WholeSaleQty { get; set; }
        public decimal? WholeSalePrice { get; set; }
        public string Location { get; set; }
        public int ShopId { get; set; }
        public int LoggedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Min { get; set; }
        public string Signal { get; set; }
        public string Code { get; set; }
        public int Max { get; set; }
        public DateTime? DeletedAt { get; set; }
        public decimal Total { get; set; }
        public Category Category { get; set; }
    }

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameFr { get; set; }
    public string DescriptionFr { get; set; }
    public string BusinessId { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public string ImageName { get; set; }
    public int LoggedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CategoryId { get; set; }
    public string Type { get; set; }
    public string Parent { get; set; }
}



public class Filter
{
    public string SearchQuery { get; set; }
    public Category SelectedCategory { get; set; }
    public string SortBy { get; set; }
}

public class Data
{
    public List<Category> LocationCategories { get; set; }
    public List<Category> Categories { get; set; }
    public List<Product> Products { get; set; }
    public Pagination Pagination { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Filter Filters { get; set; }
    public Data Data { get; set; }
}



