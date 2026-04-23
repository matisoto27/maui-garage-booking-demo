using Demo.Shared.Models;

namespace Demo.Api.Data;

public class FakeDataStore
{
    public List<Usuario> Usuarios { get; } = new();

    public List<Cochera> Cocheras { get; } = new();

    public List<Reserva> Reservas { get; } = new();

    public FakeDataStore()
    {
        Usuarios.Add(new Usuario
        {
            Id = 1,
            Dni = "12341234",
            Contrasena = "Demo1234",
            Nombre = "Usuario",
            Apellido = "Cliente",
            Telefono = "001 555 0001"
        });

        Usuarios.Add(new Usuario
        {
            Id = 2,
            Dni = "11223344",
            Contrasena = "Demo1234",
            Nombre = "Juan",
            Apellido = "Pérez",
            Telefono = "001 555 1234"
        });

        Cocheras.Add(new Cochera
        {
            Id = 1,
            UsuarioId = 2,
            CallePrincipal = "Calle 1",
            Altura = 1234,
            EntreCallePrimera = "Calle 2",
            EntreCalleSegunda = "Calle 3",
            Precio = 2500,
            Latitud = -32.943950,
            Longitud = -60.651200
        });
    }
}
