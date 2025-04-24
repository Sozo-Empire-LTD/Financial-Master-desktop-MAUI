using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Models;
using MauiApp1.Views;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using Microsoft.Maui.Storage;


namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private bool _isPasswordVisible = false;

        public MainPage()
        {
            InitializeComponent();

            // Set default headers
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)"); // Mimic a browser request
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = entryEmail.Text;
            string password = entryPassword.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            string encodedEmail = Uri.EscapeDataString(email);
            string encodedPassword = Uri.EscapeDataString(password);
            string url = $"http://127.0.0.1:8000/api/login?email={encodedEmail}&password={encodedPassword}";
           


            //string json = JsonConvert.SerializeObject(loginData);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");


            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

          

                // Log status code
                Console.WriteLine($"Response Status Code: {response.StatusCode}");

                // Read response body
                string responseBody = await response.Content.ReadAsStringAsync();
              

                if (response.IsSuccessStatusCode)
                {
                    if (responseBody != null)
                    {
                        LoginResponse? apiResponse = JsonConvert.DeserializeObject<LoginResponse>(responseBody);
                        if (apiResponse != null)
                        {
                            User user = apiResponse.Data.User; // Return User model
                            string token = apiResponse.Data.Token; // Return token
                            List<Business> businesses;
                            if (apiResponse.Data.Businesses != null)
                            {
                                businesses = apiResponse.Data.Businesses;
                            }
                                
                            else
                            {
                                businesses = new List<Business>();
                            }


                                   

                            // ✅ Save token securely
                            Preferences.Set("AuthToken", token);

                            if (businesses != null && businesses.Count > 0)
                            {
                                // Navigate to HomePage (replace with your actual home page)
                                //await Navigation.PushAsync(new OrganizationsPage(businesses));
                                //await Shell.Current.GoToAsync($"OrganizationsPage?businesses={Uri.EscapeDataString(JsonConvert.SerializeObject(businesses))}");

                                var businessesJson = Uri.EscapeDataString(JsonConvert.SerializeObject(businesses));
                                await Shell.Current.GoToAsync($"//OrganizationsPage?businesses={businessesJson}");
                            }// 
                        }
                    }
                   

                   

                   
                   
                }
                else
                {
                    await DisplayAlert("Response", responseBody, "OK");
                    Console.WriteLine(responseBody);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not connect to server. Please try again later.", "OK");
                Console.WriteLine(ex.Message);
            }
        }

        private void OnTogglePassword(object sender, EventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            entryPassword.IsPassword = !_isPasswordVisible;
        }
        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///registerPage");
        }
        private async void OnForgotPassword(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///registerPage");
        }
    }

}
