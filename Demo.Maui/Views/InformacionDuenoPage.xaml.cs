using Demo.Maui.ViewModels;

namespace Demo.Maui.Views;

public partial class InformacionDuenoPage : ContentPage
{
    private readonly InformacionDuenoViewModel _viewModel;

    public InformacionDuenoPage(InformacionDuenoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}
