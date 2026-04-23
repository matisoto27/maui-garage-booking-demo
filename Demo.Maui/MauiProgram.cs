using Microsoft.Extensions.Logging;
using Demo.Maui.Services;
using Demo.Maui.ViewModels;
using Demo.Maui.Views;

namespace Demo.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiMaps()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddHttpClient<UsuarioApiService>(client =>
		{
			string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
				? "http://10.0.2.2:5113/"
				: "http://localhost:5113/";
			client.BaseAddress = new Uri(baseUrl);
		});
		builder.Services.AddTransient<IniciarSesionPage>();
		builder.Services.AddTransient<IniciarSesionViewModel>();
		builder.Services.AddTransient<InicioPage>();
		builder.Services.AddTransient<InicioViewModel>();
		builder.Services.AddTransient<MapaPage>();
		builder.Services.AddTransient<MapaViewModel>();
		builder.Services.AddTransient<InformacionDuenoPage>();
		builder.Services.AddTransient<InformacionDuenoViewModel>();
		builder.Services.AddTransient<SolicitarReservaPage>();
		builder.Services.AddTransient<SolicitarReservaViewModel>();
		builder.Services.AddTransient<EsperarConfirmacionPage>();
		builder.Services.AddTransient<EsperarConfirmacionViewModel>();
		builder.Services.AddTransient<ReservaClientePage>();
		builder.Services.AddTransient<ReservaClienteViewModel>();

		return builder.Build();
	}
}
