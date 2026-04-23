using Demo.Maui.ViewModels;

namespace Demo.Maui.Views;

public partial class SolicitarReservaPage : ContentPage
{
    private readonly SolicitarReservaViewModel _viewModel;

    public SolicitarReservaPage(SolicitarReservaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
