using Demo.Maui.ViewModels;

namespace Demo.Maui.Views;

public partial class EsperarConfirmacionPage : ContentPage
{
    public EsperarConfirmacionPage(EsperarConfirmacionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is EsperarConfirmacionViewModel vm)
        {
            vm.DetenerEscucha();
        }
    }
}
