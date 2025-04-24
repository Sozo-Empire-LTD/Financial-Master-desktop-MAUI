using MauiApp1.ViewModels;
using MauiApp1.Views;
using MauiApp1.Views.products;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;



namespace MauiApp1;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .ConfigureSyncfusionCore()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                
            });

		builder.Services.AddTransient<OrganizationsPage>();
		builder.Services.AddTransient<OrganizationsViewModel>();

        builder.Services.AddTransient<ProductsPage>();
        builder.Services.AddSingleton<ProductViewModel>();
		

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
