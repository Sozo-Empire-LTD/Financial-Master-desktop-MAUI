using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Newtonsoft.Json; // Add this using statement

namespace MauiApp1.ViewModels
{
    [QueryProperty(nameof(BusinessesJson), "businesses")]
    public partial class OrganizationsViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<Business> businessList = new();

        [ObservableProperty]
        private ObservableCollection<Business> organizations = new();

        public ICommand ItemTappedCommand { get; }

        public string BusinessesJson
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    try // Add a try-catch block for error handling
                    {
                        BusinessList = JsonConvert.DeserializeObject<List<Business>>(Uri.UnescapeDataString(value)) ?? new List<Business>();
                        Organizations = new ObservableCollection<Business>(BusinessList);
                    }
                    catch (JsonException ex) // Catch JSON deserialization errors
                    {
                        // Handle the error appropriately (log it, show a message, etc.)
                        System.Diagnostics.Debug.WriteLine($"Error deserializing JSON: {ex.Message}");
                        BusinessList = new List<Business>(); // Important: Initialize to avoid null issues
                        Organizations = new ObservableCollection<Business>();
                    }
                }
                else
                {
                    BusinessList = new List<Business>();
                    Organizations = new ObservableCollection<Business>();
                }
            }
        }

        public OrganizationsViewModel()
        {
            ItemTappedCommand = new Command<Business>(OnItemTapped);
        }

        private async void OnItemTapped(Business selectedBusiness)
        {
            if (selectedBusiness == null) return;

            // ✅ Save token securely
            Preferences.Set("businessId", selectedBusiness.Id);
            await Shell.Current.GoToAsync("DashboardPage");

            //await Shell.Current.GoToAsync($"dashboard?businessId={selectedBusiness.Id}&businessName={selectedBusiness.Name}");
        }
    }
}
