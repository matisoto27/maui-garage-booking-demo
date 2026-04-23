using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo.Maui.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Demo.Maui.ViewModels;

public partial class ReservaClienteViewModel : ObservableObject, IQueryAttributable
{
    private const string FormaPagoDemo = "Dinero virtual";
    private const string MensajeReservaFin =
        "La reserva ha finalizado. ¡Gracias por poner a prueba la demo de esta aplicación!";

    private readonly UsuarioApiService _usuarioApiService;
    private int _idCochera;
    private string _horaIngresoParam = "08:15:00";
    private string? _montoTotalParam;
    private bool _temporizadorIniciado;
    private bool _cierreSolicitado;
    private IDispatcherTimer? _temporizador;
    private DateTime _ingresoDia;
    private DateTime _egresoDia;
    private DateTime _simuladoAhora;

    [ObservableProperty]
    private bool estaCargando = true;

    [ObservableProperty]
    private ReservaClienteModel reserva = new();

    [ObservableProperty]
    private string? horaIngreso;

    [ObservableProperty]
    private string? horaEgreso;

    [ObservableProperty]
    private bool mostrarTiempoRestante = true;

    [ObservableProperty]
    private string tituloTiempoRestante = "Tiempo restante para iniciar";

    [ObservableProperty]
    private Color colorContador = Colors.Black;

    [ObservableProperty]
    private string? cuentaRegresiva;

    [ObservableProperty]
    private bool mostrarInformacionPago;

    [ObservableProperty]
    private bool mostrarTotalAPagar;

    [ObservableProperty]
    private string? totalAPagar;

    [ObservableProperty]
    private bool mostrarBotonCancelar = true;

    [ObservableProperty]
    private bool cortesiaCancelacion;

    [ObservableProperty]
    private string? cuentaRegresivaCancelar = string.Empty;

    [ObservableProperty]
    private bool mostrarMapaChat = true;

    public ReservaClienteViewModel(UsuarioApiService usuarioApiService)
    {
        _usuarioApiService = usuarioApiService;
    }

    [RelayCommand]
    private void VerMapa()
    {
    }

    [RelayCommand]
    private void AbrirChat()
    {
    }

    [RelayCommand]
    private void CancelarReserva()
    {
    }

    public void DetenerTemporizador()
    {
        _temporizador?.Stop();
        _temporizador = null;
        _temporizadorIniciado = false;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _montoTotalParam = null;
        if (query.TryGetValue("idCochera", out object? value) && int.TryParse(value?.ToString(), out int id))
        {
            _idCochera = id;
        }

        if (query.TryGetValue("horaIngreso", out object? horaValue) && horaValue is not null)
        {
            string? h = horaValue.ToString();
            if (!string.IsNullOrWhiteSpace(h))
            {
                _horaIngresoParam = NormalizarHoraConSegundos(Uri.UnescapeDataString(h).Trim());
            }
        }

        if (query.TryGetValue("montoTotal", out object? montoValue) && montoValue is not null)
        {
            string? m = montoValue.ToString();
            if (!string.IsNullOrWhiteSpace(m))
            {
                _montoTotalParam = Uri.UnescapeDataString(m).Trim();
            }
        }

        _ = InicializarAsync();
    }

    private static string NormalizarHoraConSegundos(string h)
    {
        string[] p = h.Split(':');
        return p.Length == 2 ? $"{h}:00" : h;
    }

    private async Task InicializarAsync()
    {
        if (TimeOnly.TryParse(_horaIngresoParam, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly toIngreso))
        {
            _ingresoDia = DateTime.Today.Add(toIngreso.ToTimeSpan());
        }
        else
        {
            _ingresoDia = DateTime.Today.Add(new TimeOnly(8, 15, 0).ToTimeSpan());
        }

        _egresoDia = _ingresoDia.AddSeconds(15);
        _simuladoAhora = _ingresoDia.AddSeconds(-15);
        HoraIngreso = _ingresoDia.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + "hs";
        HoraEgreso = _egresoDia.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + "hs";

        var modelo = new ReservaClienteModel
        {
            FormaPago = FormaPagoDemo
        };
        modelo.Dueno.Alias = "—";
        modelo.Dueno.Cvu = "—";

        try
        {
            if (_idCochera > 0)
            {
                var info = await _usuarioApiService.ObtenerInformacionDuenoAsync(_idCochera);
                if (info is not null)
                {
                    string[] partes = info.ApellidoNombre.Split(',', 2, StringSplitOptions.TrimEntries);
                    if (partes.Length >= 2)
                    {
                        modelo.Dueno.Apellido = partes[0];
                        modelo.Dueno.Nombre = partes[1];
                    }
                    else
                    {
                        modelo.Dueno.Apellido = info.ApellidoNombre;
                        modelo.Dueno.Nombre = string.Empty;
                    }

                    modelo.Dueno.Dni = info.Dni;
                    modelo.Dueno.Telefono = info.Telefono;
                }
            }

            if (!string.IsNullOrWhiteSpace(_montoTotalParam)
                && decimal.TryParse(_montoTotalParam, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal totalAcordado))
            {
                modelo.TotalAPagar = decimal.Round(totalAcordado, 2, MidpointRounding.AwayFromZero)
                    .ToString("0.##", CultureInfo.InvariantCulture);
            }
            else
            {
                var resumen = _idCochera > 0
                    ? await _usuarioApiService.ObtenerResumenReservaAsync(_idCochera)
                    : null;
                if (resumen is not null)
                {
                    decimal total = resumen.PrecioPorHora * (15m / 3600m);
                    modelo.TotalAPagar = decimal.Round(total, 2, MidpointRounding.AwayFromZero)
                        .ToString("0.##", CultureInfo.InvariantCulture);
                }
                else
                {
                    modelo.TotalAPagar = "0";
                }
            }

            Reserva = modelo;
        }
        catch
        {
            Reserva = modelo;
        }
        finally
        {
            EstaCargando = false;
            IniciarTemporizador();
        }
    }

    private void IniciarTemporizador()
    {
        if (_temporizadorIniciado) return;
        if (Application.Current is null) return;
        _temporizadorIniciado = true;
        _temporizador = Application.Current.Dispatcher.CreateTimer();
        _temporizador.Interval = TimeSpan.FromSeconds(1);
        _temporizador.Tick += OnTickTemporizador;
        ActualizarContador();
        _temporizador.Start();
    }

    private void OnTickTemporizador(object? sender, EventArgs e)
    {
        _simuladoAhora = _simuladoAhora.AddSeconds(1);
        MainThread.BeginInvokeOnMainThread(ActualizarContador);
    }

    private void ActualizarContador()
    {
        if (_simuladoAhora >= _egresoDia)
        {
            _ = FinalizarHaciaInicioAsync();
            return;
        }

        if (_simuladoAhora < _ingresoDia)
        {
            TituloTiempoRestante = "Tiempo restante para iniciar";
            CuentaRegresiva = FormatearResto(_ingresoDia - _simuladoAhora);
        }
        else
        {
            TituloTiempoRestante = "Tiempo restante para finalizar";
            CuentaRegresiva = FormatearResto(_egresoDia - _simuladoAhora);
        }

        ColorContador = Colors.Black;
    }

    private static string FormatearResto(TimeSpan resto) =>
        (resto < TimeSpan.Zero ? TimeSpan.Zero : resto).ToString("hh\\:mm\\:ss", CultureInfo.InvariantCulture);

    private async Task FinalizarHaciaInicioAsync()
    {
        if (_cierreSolicitado) return;
        _cierreSolicitado = true;
        DetenerTemporizador();
        const string mensaje = MensajeReservaFin;
        if (Application.Current is null) return;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            Page? page = Application.Current!.Windows.Count > 0
                ? Application.Current.Windows[0].Page
                : null;
            if (page is not null)
            {
                try
                {
                    await page.DisplayAlert("Reserva finalizada", mensaje, "OK");
                }
                catch
                {
                }
            }

            if (Shell.Current is not null)
            {
                await Shell.Current.GoToAsync("//Inicio");
            }
        });
    }
}

public sealed class ReservaClienteModel
{
    public string FormaPago { get; set; } = string.Empty;
    public string TotalAPagar { get; set; } = "0";
    public DuenoClienteModel Dueno { get; } = new();
}

public sealed class DuenoClienteModel
{
    public string Apellido { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public string Cvu { get; set; } = string.Empty;
}
