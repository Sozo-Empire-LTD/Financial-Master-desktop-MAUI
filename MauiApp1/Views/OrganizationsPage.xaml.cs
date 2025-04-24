using MauiApp1.Models;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class OrganizationsPage : ContentPage
{
    public OrganizationsPage(OrganizationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
