using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MauiApp1.Models;

public class ProductApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://your-api.com/api/products"; // Replace with your actual API URL
  

    public ProductApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
       
    }

    /// <summary>
    /// Attaches Authorization Header to HTTP Requests
    /// </summary>
    
    /// <summary>
    /// Get product by ID
    /// </summary>
    public async Task<Product> GetProductByIdAsync(int id)
    {

        string token = Preferences.Get("AuthToken", string.Empty);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to fetch product");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Product>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    public async Task<bool> CreateProductAsync(Product product)
    {
        string token = Preferences.Get("AuthToken", string.Empty);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        var jsonContent = JsonSerializer.Serialize(product);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(BaseUrl, httpContent);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    public async Task<bool> UpdateProductAsync(Product product)
    {

        string token = Preferences.Get("AuthToken", string.Empty);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        var jsonContent = JsonSerializer.Serialize(product);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{BaseUrl}/{product.Id}", httpContent);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Delete a product by ID
    /// </summary>
    public async Task<bool> DeleteProductAsync(int id)
    {

        string token = Preferences.Get("AuthToken", string.Empty);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }
}
