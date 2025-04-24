namespace MauiApp1.Views.products;
using MauiApp1.ViewModels;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

[QueryProperty(nameof(productSlug), "productSlug")]
public partial class ViewProductPage : ContentPage
{
    private string _productSlug;

    //public ICommand EditProductCommand { get; }
    public string productSlug
    {
        get => _productSlug;
        set => _productSlug = value;
    }

    public ViewProductPage()
    {
        InitializeComponent();
        BindingContext = new ProductDetailViewModel(); // Set the BindingContext here  
        this.NavigatedTo += ViewProductPage_NavigatedTo;
        Debug.WriteLine($"the slug from view to edit: {productSlug}");
        //EditProductCommand = new RelayCommand<string>(async (productSlug) => await ExecuteEditProduct(productSlug));
    }

    public async void  MoveToEditProductPage(object sender, EventArgs e)
    {
        //Debug.WriteLine($"the slug from view to edit 111: {productSlug}");
        await Shell.Current.GoToAsync($"///EditProductPage?productSlug={productSlug}", true);
    }


    private async void ViewProductPage_NavigatedTo(object sender, NavigatedToEventArgs args)
    {
        if (BindingContext is ProductDetailViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(productSlug))
            {
                Debug.WriteLine($"productSlug131:{productSlug}");
                await viewModel.LoadProductDetails(productSlug);
            }
            else
            {
                // Handle the case where productSlug is not passed or is null  
                Debug.WriteLine("productSlug was not passed or is null");
            }
        }
    }

   
}
