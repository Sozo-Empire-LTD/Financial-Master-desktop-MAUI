using MauiApp1.ViewModels;

namespace MauiApp1.Views.products;

public partial class ProductsPage : ContentPage
{
    private readonly ProductViewModel _viewModel;
    public ProductsPage(ProductViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
       
        BindingContext = _viewModel;

        // Call LoadProducts to populate the data when the page appears
        this.Appearing += ProductsPage_Appearing;
    }
    private async void ProductsPage_Appearing(object sender, EventArgs e)
    {
        try
        {
            await _viewModel.LoadProducts();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load products: {ex.Message}", "OK");
        }
    }

    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///CreateProductPage");

    }



}