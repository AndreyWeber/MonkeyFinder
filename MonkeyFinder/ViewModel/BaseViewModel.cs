namespace MonkeyFinder.ViewModel;

public partial class BaseViewModel : ObservableObject
{
#pragma warning disable MVVMTK0045  // "AOT not compatible" warning
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string? title;

    public bool IsNotBusy => !IsBusy;
#pragma warning restore MVVMTK0045

    public BaseViewModel()
    {

    }
}
