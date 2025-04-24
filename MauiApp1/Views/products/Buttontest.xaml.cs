namespace MauiApp1.Views.products;

public partial class Buttontest : ContentPage
{
	public Buttontest()
	{
		InitializeComponent();

	}
    private void Frame_Tapped(object sender, System.EventArgs e)
    {
        ButtonContainer.IsVisible = !ButtonContainer.IsVisible;
    }

    private void EditButton_Clicked(object sender, System.EventArgs e)
    {
        // Handle Edit button click
        DisplayAlert("Edit", "Edit button clicked", "OK");
    }

    private void ViewButton_Clicked(object sender, System.EventArgs e)
    {
        // Handle View button click
        DisplayAlert("View", "View button clicked", "OK");
    }

    private void SellButton_Clicked(object sender, System.EventArgs e)
    {
        // Handle Sell button click
        DisplayAlert("Sell", "Sell button clicked", "OK");
    }

    private void SoftDeleteButton_Clicked(object sender, System.EventArgs e)
    {
        // Handle Soft delete button click
        DisplayAlert("Soft Delete", "Soft delete button clicked", "OK");
    }

    private void DeleteButton_Clicked(object sender, System.EventArgs e)
    {
        // Handle Delete button click
        DisplayAlert("Delete", "Delete button clicked", "OK");
    }

}