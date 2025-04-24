using MauiApp1.Views;
using MauiApp1.Views.products;
using MauiApp1.Views.sale;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

           Routing.RegisterRoute(nameof(OrganizationsPage), typeof(OrganizationsPage));
           Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage)); 
           Routing.RegisterRoute(nameof(ProductsPage), typeof(ProductsPage));
           Routing.RegisterRoute(nameof(ViewProductPage), typeof(ViewProductPage));
           Routing.RegisterRoute(nameof(CreateProductPage), typeof(CreateProductPage));
           Routing.RegisterRoute("EditProductPage", typeof(EditProductPage));
           Routing.RegisterRoute(nameof(Buttontest), typeof(Buttontest));
           Routing.RegisterRoute(nameof(CreateSalePage), typeof(CreateSalePage));
           Routing.RegisterRoute(nameof(SalePage), typeof(SalePage));
           Routing.RegisterRoute(nameof(SaleTablePage), typeof(SaleTablePage));


        }
    }
}
