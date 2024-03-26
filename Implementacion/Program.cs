using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

public static class GlobalVariables
{
    public static string GraphQLUrl { get; set; }
    public static string Token { get; set; }
}

public class GraphQLService
{
    private readonly GraphQLHttpClient _graphQLClient;

    public GraphQLService(string graphqlUrl)
    {
        _graphQLClient = new GraphQLHttpClient(graphqlUrl, new NewtonsoftJsonSerializer());
    }

    public async Task<dynamic> Login(string shop, string email, string password)
    {
        var loginRequest = new GraphQLRequest
        {
            Query = @"
                mutation LOGIN($shop: String!, $email: String!, $password: String!) {
                    login(shop: $shop, email: $email, password: $password) {
                        token
                        refreshToken
                        perfil {
                            nombres
                            apellidos
                            avatar
                            cargo
                            ci
                            correo
                            rol
                            sigla
                            dominio
                            tipo
                            vigente
                            sucursal {
                                codigo
                                direccion
                                telefono
                                departamento {
                                    codigo
                                    codigoPais
                                    sigla
                                    departamento
                                }
                            }
                            puntoVenta {
                                codigo
                                descripcion
                                nombre
                                tipoPuntoVenta {
                                    codigoClasificador
                                    descripcion
                                }
                            }
                            actividadEconomica {
                                codigoCaeb
                                descripcion
                                tipoActividad
                            }
                        }
                    }
                }",
            Variables = new
            {
                shop,
                email,
                password
            }
        };

        var response = await _graphQLClient.SendMutationAsync<dynamic>(loginRequest);

        if (response.Errors != null && response.Errors.Length > 0)
        {
            foreach (var error in response.Errors)
            {
                Console.WriteLine($"GraphQL Error: {error.Message}");
            }
            return null;
        }
        else
        {
            // Guardar el token en la variable global
            GlobalVariables.Token = response.Data.login.token;
            //Console.WriteLine(GlobalVariables.Token);
            return response.Data.login;
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        // Asignar la URL GraphQL a la variable global
        GlobalVariables.GraphQLUrl = "https://sandbox.isipass.net/api";

        var graphQLService = new GraphQLService(GlobalVariables.GraphQLUrl);
        var loginData = await graphQLService.Login("sandbox", "nick077n@gmail.com", "13969594");

        if (loginData != null)
        {
            //Console.WriteLine($"Token: {loginData.token}");
            //Console.WriteLine($"Refresh Token: {loginData.refreshToken}");
            //Console.WriteLine($"Perfil - Nombres: {loginData.perfil.nombres}");
            //Console.WriteLine($"Perfil - Apellidos: {loginData.perfil.apellidos}");

            // Puedes acceder a otras propiedades de loginData aquí
        }
        else
        {
            Console.WriteLine("Error en la autenticación.");
        }
        //await queryFacturas.ConsultarFacturas(true, 1, 0, 0);

        dynamic input = ObtenerDatosParaFactura();
        await MutationFacturas.RegistrarFacturaCompraVentaOnline(input);
    }
    static dynamic ObtenerDatosParaFactura()
    {
        dynamic input = new System.Dynamic.ExpandoObject();

        // Aquí llenas el objeto input con los datos necesarios
        // Supongamos que los datos vienen de la entrada del usuario

        // Datos del cliente
        input.cliente = new System.Dynamic.ExpandoObject();
        input.cliente.codigoCliente = "6186816017";
        input.cliente.razonSocial = "NIT inexistente";
        input.cliente.numeroDocumento = "6176816017";
        input.cliente.email = "richi617@gmail.com";
        input.cliente.codigoTipoDocumentoIdentidad = 5;

        // Otros datos de la factura
        input.codigoExcepcion = 1;
        input.actividadEconomica = "620000";
        input.codigoMetodoPago = 1;
        input.descuentoAdicional = 0;
        input.codigoMoneda = 1;
        input.tipoCambio = 1;
        input.detalleExtra = "<p><strong>Detalle extra</strong></p>";

        // Detalle de la factura
        input.detalle = new dynamic[1];
        input.detalle[0] = new System.Dynamic.ExpandoObject();
        input.detalle[0].codigoProductoSin = "83131";
        input.detalle[0].codigoProducto = "01010101004";
        input.detalle[0].descripcion = "POLLO SIN MENUDENCIA H-C";
        input.detalle[0].cantidad = 10;
        input.detalle[0].unidadMedida = 1;
        input.detalle[0].precioUnitario = 13.40;
        input.detalle[0].montoDescuento = 133;

        return input;
    }

}

