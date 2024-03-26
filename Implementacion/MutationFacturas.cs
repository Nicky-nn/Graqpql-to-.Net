using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;

public class GraphQLMutationService
{
    private readonly GraphQLHttpClient _graphQLClient;

    public GraphQLMutationService(string graphqlUrl, string token)
    {
        _graphQLClient = new GraphQLHttpClient(graphqlUrl, new NewtonsoftJsonSerializer());
        _graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<dynamic> MutationFacturaCompraVentaCreate(dynamic input)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                mutation FCV_REGISTRO_ONLINE($input: FacturaCompraVentaInput!) {
                    facturaCompraVentaCreate(
                        entidad: { codigoSucursal: 0, codigoPuntoVenta: 0 }
                        input: $input
                    ) {
                        _id
                        cafc
                        representacionGrafica {
                            pdf
                            rollo
                            sin
                            xml
                        }
                        cliente {
                            nombres
                            numeroDocumento
                            razonSocial
                            state
                            tipoDocumentoIdentidad {
                                codigoClasificador
                                descripcion
                            }
                            
                            UpdatedAt
                            usucre
                            usumod
                        }
                        state
                    }
                }",
            Variables = new
            {
                input
            }
        };

        var response = await _graphQLClient.SendMutationAsync<dynamic>(request);

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
            var data = JObject.FromObject(response.Data);
            var pdfUrl = (string)data["facturaCompraVentaCreate"]["representacionGrafica"]["rollo"];

            // Abrir la URL en el navegador
            try
            {
                Process.Start(new ProcessStartInfo(pdfUrl) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir la URL en el navegador: {ex.Message}");
            }

            return response.Data;
        }
    }
}

class MutationFacturas
{
    public static async Task RegistrarFacturaCompraVentaOnline(dynamic input)
    {
        // Suponiendo que tienes la URL y el token almacenados en variables globales
        var graphqlUrl = GlobalVariables.GraphQLUrl;
        var token = GlobalVariables.Token;

        var mutationService = new GraphQLMutationService(graphqlUrl, token);
        var facturaCompraVentaData = await mutationService.MutationFacturaCompraVentaCreate(input);

        if (facturaCompraVentaData != null)
        {
            // Procesar los datos de la facturaCompraVentaData aquí
            Console.WriteLine("Factura registrada exitosamente:");
            Console.WriteLine(facturaCompraVentaData);
        }
        else
        {
            Console.WriteLine("Error en el registro de factura.");
        }
    }
}
