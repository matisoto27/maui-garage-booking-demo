using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo.Maui.Services;
using Microsoft.Maui.Controls;

namespace Demo.Maui.ViewModels;

public partial class EsperarConfirmacionViewModel : ObservableObject, IQueryAttributable
{
    private readonly UsuarioApiService _usuarioApiService;
    private int _idCochera;
    private string _horaIngreso = "08:15";
    private string _montoTotal = string.Empty;
    private CancellationTokenSource? _cts;
    private IDispatcherTimer? _temporizadorRedireccion;
    private bool _redireccionRealizada;

    [ObservableProperty]
    private bool confirmada;

    [ObservableProperty]
    private string textoTiempoLimite = "Tiempo transcurrido 0s";

    [ObservableProperty]
    private string direccionCompleta = "Dirección no disponible";

    [ObservableProperty]
    private string ingresoMenosCinco = "a las --:--hs";

    [ObservableProperty]
    private string textoCuentaRegresiva = string.Empty;

    public EsperarConfirmacionViewModel(UsuarioApiService usuarioApiService)
    {
        _usuarioApiService = usuarioApiService;
    }

    [RelayCommand]
    private async Task Siguiente()
    {
        await IrAReservaClienteAsync();
    }

    [RelayCommand]
    private void CancelarReserva()
    {
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _montoTotal = string.Empty;
        if (query.TryGetValue("idCochera", out object? value) && int.TryParse(value?.ToString(), out int id))
        {
            _idCochera = id;
        }
        if (query.TryGetValue("horaIngreso", out object? horaValue) && horaValue is not null)
        {
            string? raw = horaValue.ToString();
            if (!string.IsNullOrWhiteSpace(raw))
            {
                _horaIngreso = Uri.UnescapeDataString(raw).Trim();
            }
        }

        if (query.TryGetValue("montoTotal", out object? montoValue) && montoValue is not null)
        {
            string? m = montoValue.ToString();
            if (!string.IsNullOrWhiteSpace(m))
            {
                _montoTotal = Uri.UnescapeDataString(m).Trim();
            }
        }

        await InicializarAsync();
    }

    public void DetenerEscucha()
    {
        _temporizadorRedireccion?.Stop();
        _temporizadorRedireccion = null;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public async Task InicializarAsync()
    {
        DetenerEscucha();
        _redireccionRealizada = false;
        _cts = new CancellationTokenSource();
        CancellationToken token = _cts.Token;

        Confirmada = false;
        TextoTiempoLimite = "Tiempo transcurrido 0s";
        TextoCuentaRegresiva = string.Empty;

        if (_idCochera > 0)
        {
            var resumen = await _usuarioApiService.ObtenerResumenReservaAsync(_idCochera);
            if (resumen is not null)
            {
                DireccionCompleta = resumen.DireccionCompleta;
            }
        }

        if (TryParseHoraSoloDia(_horaIngreso, out TimeOnly horaIngreso))
        {
            IngresoMenosCinco = $"a las {horaIngreso.AddMinutes(-5):HH:mm}hs";
        }
        else
        {
            IngresoMenosCinco = "a las --:--hs";
        }

        for (int i = 1; i <= 5; i++)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            TextoTiempoLimite = $"Tiempo transcurrido {i}s";
            await Task.Delay(1000, token);
        }

        if (!token.IsCancellationRequested)
        {
            Confirmada = true;
            if (TryParseHoraSoloDia(_horaIngreso, out TimeOnly t))
            {
                IngresoMenosCinco = $"a las {t.AddMinutes(-5):HH:mm}hs";
            }

            IniciarContadorRedireccionamiento();
        }
    }

    private void IniciarContadorRedireccionamiento()
    {
        _temporizadorRedireccion?.Stop();
        if (Application.Current?.Dispatcher is null)
        {
            return;
        }

        int cuentaRegresiva = 10;
        _temporizadorRedireccion = Application.Current.Dispatcher.CreateTimer();
        _temporizadorRedireccion.Interval = TimeSpan.FromSeconds(1);
        _temporizadorRedireccion.Tick += async (_, _) =>
        {
            TextoCuentaRegresiva = $"En {cuentaRegresiva--} segundos serás redirigido";
            if (cuentaRegresiva < 0 && !_redireccionRealizada)
            {
                await IrAReservaClienteAsync();
            }
        };
        _temporizadorRedireccion.Start();
    }

    private static bool TryParseHoraSoloDia(string? texto, out TimeOnly hora)
    {
        hora = default;
        if (string.IsNullOrWhiteSpace(texto))
        {
            return false;
        }

        string s = Uri.UnescapeDataString(texto.Trim());
        if (TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out hora))
        {
            return true;
        }

        string[] formatos = ["HH:mm:ss", "H:mm:ss", "HH:mm", "H:mm"];
        foreach (string f in formatos)
        {
            if (TimeOnly.TryParseExact(s, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out hora))
            {
                return true;
            }
        }

        return false;
    }

    private async Task IrAReservaClienteAsync()
    {
        if (_redireccionRealizada || !Confirmada)
        {
            return;
        }

        _redireccionRealizada = true;
        DetenerEscucha();

        string hora = _horaIngreso.Trim();
        if (hora.Split(':').Length == 2)
        {
            hora += ":00";
        }

        string ruta = $"//ReservaCliente?idCochera={_idCochera}&horaIngreso={Uri.EscapeDataString(hora)}";
        if (!string.IsNullOrWhiteSpace(_montoTotal))
        {
            ruta += $"&montoTotal={Uri.EscapeDataString(_montoTotal)}";
        }

        await Shell.Current.GoToAsync(ruta);
    }
}
