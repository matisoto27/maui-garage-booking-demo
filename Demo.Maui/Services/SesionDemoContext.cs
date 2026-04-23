namespace Demo.Maui.Services;

public static class SesionDemoContext
{
    public static int IdUsuario { get; private set; } = 1;

    public static void EstablecerUsuario(int idUsuario) => IdUsuario = idUsuario > 0 ? idUsuario : 1;
}
