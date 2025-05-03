namespace MonkeyFinder.View;

public partial class MainPage : ContentPage
{
    private readonly MonkeysViewModel _viewModel;

    public MainPage(MonkeysViewModel viewModel)
    {
	    InitializeComponent();

        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await GetPermissionsAsync();
    }

    private async Task GetPermissionsAsync()
    {
        try
        {
            if (_viewModel.EnableLocationFeaturesCommand.CanExecute(null))
            {
                await _viewModel.EnableLocationFeaturesCommand.ExecuteAsync(null);
            }

            if (_viewModel.EnablePostNotificationsCommand.CanExecute(null))
            {
                await _viewModel.EnablePostNotificationsCommand.ExecuteAsync(null);
            }
        }
        catch (PermissionException ex)
        {
            Debug.WriteLine(ex);
        }
    }
}