using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.Models;
using MauiApp1.Views.products;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syncfusion.Maui.Core.Carousel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MauiApp1
{

    public partial class DashboardPage : ContentPage, INotifyPropertyChanged
    {


        private readonly HttpClient _httpClient;
        private const string ApiUrl = "http://127.0.0.1:8000/api/dashboard-details/";
        public ObservableCollection<Transaction> Transactions { get; set; }

        private ObservableCollection<ChartData> _purchases = new ObservableCollection<ChartData>();
        private ObservableCollection<ChartData> _sales = new ObservableCollection<ChartData>();
        private ObservableCollection<ChartData> _profits = new ObservableCollection<ChartData>();

        
        


        public ObservableCollection<ChartData> Purchases
        {
            get => _purchases;
            set { _purchases = value; OnPropertyChanged(nameof(Purchases)); }
        }

        public ObservableCollection<ChartData> Sales
        {
            get => _sales;
            set { _sales = value; OnPropertyChanged(nameof(Sales)); }
        }

        public ObservableCollection<ChartData> Profits
        {
            get => _profits;
            set { _profits = value; OnPropertyChanged(nameof(Profits)); }
        }



        public ObservableCollection<string> Options { get; set; }
       

        private string _selectedOption;
        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (_selectedOption != value)
                {
                    _selectedOption = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedOption)));
                }
            }
        }

        // Add properties for binding the values
        private int _value1;
        private decimal _value2;
        private decimal _value3;
        private decimal _value4;

        public int Value1
        {
            get => _value1;
            set
            {
                _value1 = value;
                OnPropertyChanged();
            }
        }

        public decimal Value2
        {
            get => _value2;
            set
            {
                _value2 = value;
                OnPropertyChanged();
            }
        }

        public decimal Value3
        {
            get => _value3;
            set
            {
                _value3 = value;
                OnPropertyChanged();
            }
        }

        public decimal Value4
        {
            get => _value4;
            set
            {
                _value4 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public DashboardPage()
        {
            InitializeComponent();

            _httpClient = new HttpClient();
            
            Transactions = new ObservableCollection<Transaction>();

            for (int i = 0; i < Transactions.Count; i++)
            {
                Transactions[i].IsLastItem = (i == Transactions.Count - 1);
            }

            // Fetch dashboard details from API
            FetchDashboardDetails();

            FetchWeeklyData(); // Default to weekly data

            Options = new ObservableCollection<string>
            {
                 "Monday", "Tuesday","Wednesday", "Thursday", "Friday", "Sunday", "Saturday"
            };

            BindingContext = this;
        }

        private async void OnProductsClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new ProductsPage());
            
                //await mainPage.Navigation.PushAsync(new ProductsPage(viewModel));
                await Shell.Current.GoToAsync("ProductsPage");
            
        }


        private async void FetchDashboardDetails()
        {
            try
            {
                string token = Preferences.Get("AuthToken", string.Empty);
                int businessId = Preferences.Get("businessId", -1);

                if (string.IsNullOrEmpty(token))
                {
                    await DisplayAlert("Error", "Authentication token not found. Please log in again.", "OK");
                    return;
                }

                string requestUrl = $"{ApiUrl}{businessId}";

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserialize response
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
                    if (apiResponse?.Data != null)
                    {

                        // Update the binding properties
                        Value1 = apiResponse.Data.Value1;
                        Value2 = apiResponse.Data.Value2;
                        Value3 = apiResponse.Data.Value3;
                        Value4 = apiResponse.Data.Value4;

                        if (apiResponse?.Data?.Transactions != null)
                        {
                            Transactions.Clear();
                            foreach (var transaction in apiResponse.Data.Transactions)
                            {
                                Transactions.Add(transaction);
                            }
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to load dashboard details", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        public async Task FetchWeeklyData( int value = 7)
        {
            string token = Preferences.Get("AuthToken", string.Empty);
            int businessId = Preferences.Get("businessId", -1);

            if (string.IsNullOrEmpty(token))
            {
                await DisplayAlert("Error", "Authentication token not found. Please log in again.", "OK");
                return;
            }
            string url = value == 7 ? $"/products/profits_week/{businessId}" : $"/products/profits_month/{businessId}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetStringAsync($"http://127.0.0.1:8000/api{url}");
               
                var items = JsonConvert.DeserializeObject<ProfitResponse>(response);
                if (items != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Purchases.Clear();
                        Sales.Clear();
                        Profits.Clear();

                        // Initialize full week dataset
                        var fullWeek = new Dictionary<string, double>
                        {
                            { "Monday", 0 }, { "Tuesday", 0 }, { "Wednesday", 0 },
                            { "Thursday", 0 }, { "Friday", 0 }, { "Saturday", 0 }, { "Sunday", 0 }
                        };
                        // Fill in actual profits
                        Purchases = new ObservableCollection<ChartData>
                                    {
                                        new ChartData("Monday", 0),
                                        new ChartData("Tuesday", 15000),
                                        new ChartData("Wednesday", 20000),
                                        new ChartData("Thursday", 18000),
                                        new ChartData("Friday", 22000),
                                        new ChartData("Saturday", 25000),
                                        new ChartData("Sunday", 70000)
                                    };
                        Sales = new ObservableCollection<ChartData>
                                {
                                    new ChartData("Monday", 12000),
                                    new ChartData("Tuesday", 17000),
                                    new ChartData("Wednesday", 25000),
                                    new ChartData("Thursday", 23000),
                                    new ChartData("Friday", 26000),
                                    new ChartData("Saturday", 27000),
                                    new ChartData("Sunday", 32000)
                                };

                        Profits = new ObservableCollection<ChartData>
                                    {
                                        new ChartData("Monday", 2000),
                                        new ChartData("Tuesday", 3000),
                                        new ChartData("Wednesday", 5000),
                                        new ChartData("Thursday", 5000),
                                        new ChartData("Friday", 4000),
                                        new ChartData("Saturday", 2000),
                                        new ChartData("Sunday", 5000)
                                    };

                        //foreach (var item in items.Purchases)
                        //{


                        //    Purchases.Add(new ChartData(item.Date, item.Amount));
                        //}

                        //foreach (var item in items.Sales)
                        //{

                        //    Sales.Add(new ChartData(item.Date, item.Amount));
                        //}

                        //foreach (var item in items.Profits)
                        //{

                        //    Profits.Add(new ChartData(item.Date, item.Profit));
                        //}


                    });
                }

                UpdateChart();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
        }

       
        private void UpdateChart()
        {
            
            OnPropertyChanged(nameof(Purchases));
            OnPropertyChanged(nameof(Sales));
            OnPropertyChanged(nameof(Profits));
        }

    }
    //end


    //public class SalesData
    //{
    //    public List<Stat> Purchases { get; set; }
    //    public List<Stat> Sales { get; set; }
    //    public List<Stat> Profits { get; set; }
    //}

    public class Stat
    {
        public string Date { get; set; }  // Keep as string if API returns it as string
        public double Amount { get; set; }
    }


    public class ChartData
    {
        public string Label { get; set; }
        public double Value { get; set; }

        public ChartData(string label, double value)
        {
            Label = label;
            Value = value;
        }


    }

    public class ProfitResponse
    {
        public List<PurchaseData> Purchases { get; set; }
        public List<SalesData> Sales { get; set; }
        public List<ProfitData> Profits { get; set; }
    }

    public class PurchaseData
    {
        public string Date { get; set; }
        public double Amount { get; set; }
    }

    public class SalesData
    {
        public string Date { get; set; }
        public double Amount { get; set; }
    }



    public class ProfitData
    {
        public string Date { get; set; }
        public double Profit { get; set; }
    }


// API Response Model
public class ApiResponse
    {
        public bool Success { get; set; }
        public DashboardData Data { get; set; }
    }

    public class DashboardData
    {
        public int SelectedDayNumeric { get; set; }
        public Business Business { get; set; }
        public int Value1 { get; set; }
        public decimal Value2 { get; set; }
        public string MostRecentDay { get; set; }
        public string CarbonStartOfWeek { get; set; }
        public string CarbonEndOfWeek { get; set; }
        public decimal Value3 { get; set; }
        public decimal Value4 { get; set; }
        public List<Transaction> Transactions { get; set; }
    }

    

    // Transaction Model
    public class Transaction
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int BusinessId { get; set; }
        public decimal Amount { get; set; }
        public int LoggedBy { get; set; }
        public string created_at { get; set; }
        public string UpdatedAt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Type { get; set; }
        public bool IsLastItem { get; set; } // <-- Add this
        public string FullName => $"{FirstName} {LastName}";

        // Helper property for formatted date
        public string FormattedDate
        {
            get
            {
                if (string.IsNullOrEmpty(created_at))
                {
                    return "Date not available";
                }
                return string.Format("{0:yy/MM/dd}", DateTime.Parse(created_at));
               
            }
        }

    }


}
