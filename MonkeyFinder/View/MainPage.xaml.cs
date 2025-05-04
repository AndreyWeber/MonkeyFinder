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

        await _viewModel.OnAppearingAsync();
    }
}