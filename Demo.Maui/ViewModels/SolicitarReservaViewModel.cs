using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo.Maui.Services;
using Microsoft.Maui.Controls;

namespace Demo.Maui.ViewModels;

public partial class SolicitarReservaViewModel : ObservableObject, IQueryAttributable
{
    private readonly UsuarioApiService _usuarioApiService;
    private decimal _precioPorHora;

    [ObservableProperty]
    private bool estaCargando = true;

    [ObservableProperty]
    private int idCochera;

    [ObservableProperty]
    private string horaActual = "--:--hs";

    [ObservableProperty]
    private List<string> horariosInicio = ["08:15hs", "08:30hs", "08:45hs"];

    [ObservableProperty]
    private int indiceHorarioSeleccionado = -1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectorDuracionHabilitado))]
    private List<string>? duracionesDisponibles;

    [ObservableProperty]
    private int indiceDuracionSeleccionado = -1;

    [ObservableProperty]
    private string tituloSelectorDuracion = "Seleccione una hora de ingreso";

    [ObservableProperty]
    private List<string> metodosDePago = ["Efectivo", "Dinero virtual"];

    [ObservableProperty]
    private int indiceMetodoSeleccionado;

    [ObservableProperty]
    private string precioActual = "-";

    [ObservableProperty]
    private string horaEgreso = "--:--hs";

    [ObservableProperty]
    private string montoTotal = "-";

    public SolicitarReservaViewModel(UsuarioApiService usuarioApiService)
    {
        _usuarioApiService = usuarioApiService;
    }

    public bool SelectorDuracionHabilitado => DuracionesDisponibles is { Count: > 0 };

    [RelayCommand]
    private async Task Finalizar()
    {
        if (IndiceHorarioSeleccionado < 0 || IndiceDuracionSeleccionado < 0 || IndiceMetodoSeleccionado < 0)
        {
            return;
        }

        string horaIngreso = HorariosInicio[IndiceHorarioSeleccionado].Replace("hs", string.Empty).Trim();
        if (horaIngreso.Split(':').Length == 2)
        {
            horaIngreso += ":00";
        }

        string montoQuery = MontoTotal.Replace("$", string.Empty).Trim();
        if (montoQuery == "-")
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"//EsperarConfirmacion?idCochera={IdCochera}&horaIngreso={Uri.EscapeDataString(horaIngreso)}&montoTotal={Uri.EscapeDataString(montoQuery)}");
    }

    [RelayCommand]
    private async Task Volver()
    {
        if (IdCochera > 0)
        {
            await Shell.Current.GoToAsync($"//InformacionDueno?idCochera={IdCochera}");
        }
        else
        {
            await Shell.Current.GoToAsync("//Mapa");
        }
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("idCochera", out object? value) && int.TryParse(value?.ToString(), out int id))
        {
            IdCochera = id;
        }

        await InicializarAsync();
    }

    public async Task InicializarAsync()
    {
        EstaCargando = true;
        HoraActual = "08:03hs";
        DuracionesDisponibles = null;
        IndiceDuracionSeleccionado = -1;
        TituloSelectorDuracion = "Seleccione una hora de ingreso";
        HoraEgreso = "--:--hs";
        MontoTotal = "-";
        IndiceMetodoSeleccionado = 0;

        if (IdCochera > 0)
        {
            var resumen = await _usuarioApiService.ObtenerResumenReservaAsync(IdCochera);
            if (resumen is not null)
            {
                _precioPorHora = resumen.PrecioPorHora;
                PrecioActual = $"${_precioPorHora}";
            }
            else
            {
                _precioPorHora = 0;
                PrecioActual = "-";
            }
        }

        EstaCargando = false;
    }

    partial void OnIndiceHorarioSeleccionadoChanged(int value)
    {
        RecalcularDuracionYTotales();
    }

    partial void OnIndiceDuracionSeleccionadoChanged(int value)
    {
        RecalcularHoraEgresoYMonto();
    }

    private void RecalcularDuracionYTotales()
    {
        if (IndiceHorarioSeleccionado < 0 || IndiceHorarioSeleccionado >= HorariosInicio.Count)
        {
            DuracionesDisponibles = null;
            IndiceDuracionSeleccionado = -1;
            TituloSelectorDuracion = "Seleccione una hora de ingreso";
            HoraEgreso = "--:--hs";
            MontoTotal = "-";
            return;
        }

        TimeOnly inicio = ObtenerHoraInicioSeleccionada();
        TimeOnly tope = new(9, 15);
        int minutosMaximos = (int)(tope - inicio).TotalMinutes;

        List<string> opcionesDuracion = [];
        for (int minutos = 30; minutos <= minutosMaximos; minutos += 15)
        {
            opcionesDuracion.Add(TransformarDuracion(minutos));
        }

        DuracionesDisponibles = opcionesDuracion;
        IndiceDuracionSeleccionado = 0;
        TituloSelectorDuracion = "Seleccione una duración";
        RecalcularHoraEgresoYMonto();
    }

    private TimeOnly ObtenerHoraInicioSeleccionada()
    {
        return HorariosInicio[IndiceHorarioSeleccionado] switch
        {
            "08:15hs" => new TimeOnly(8, 15),
            "08:30hs" => new TimeOnly(8, 30),
            _ => new TimeOnly(8, 45)
        };
    }

    private void RecalcularHoraEgresoYMonto()
    {
        if (IndiceHorarioSeleccionado < 0
            || DuracionesDisponibles is null
            || IndiceDuracionSeleccionado < 0
            || IndiceDuracionSeleccionado >= DuracionesDisponibles.Count)
        {
            HoraEgreso = "--:--hs";
            MontoTotal = "-";
            return;
        }

        TimeOnly inicio = ObtenerHoraInicioSeleccionada();
        int minutosDuracion = 30 + IndiceDuracionSeleccionado * 15;
        TimeOnly egreso = inicio.AddMinutes(minutosDuracion);
        HoraEgreso = $"{egreso:HH:mm}hs";

        decimal total = _precioPorHora * (minutosDuracion / 60m);
        MontoTotal = _precioPorHora > 0
            ? $"${decimal.Round(total, 2).ToString("0.##", CultureInfo.InvariantCulture)}"
            : "-";
    }

    private static string TransformarDuracion(int minutos)
    {
        if (minutos == 60)
        {
            return "1 hora";
        }

        return $"{minutos} minutos";
    }
}
