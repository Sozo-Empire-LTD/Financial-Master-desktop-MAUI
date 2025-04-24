

namespace MauiApp1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //MainPage = CreateMainPage(); // Set MainPage correctly
        }

        //private static Page CreateMainPage()
        //{
        //    return new NavigationPage(new MainPage()); // Ensure navigation works
        //}

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell()); // Use the same method
        }
    }
}
