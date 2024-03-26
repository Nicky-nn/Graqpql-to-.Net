using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

public class GraphQLQueryService
{
    private readonly GraphQLHttpClient _graphQLClient;

    public GraphQLQueryService(string graphqlUrl, string token)
    {
        _graphQLClient = new GraphQLHttpClient(graphqlUrl, new NewtonsoftJsonSerializer());
        _graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<dynamic> QueryFacturaCompraVentaAll(bool reverse, int limit, int codigoSucursal, int codigoPuntoVenta)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query FacturaCompraVenta($reverse: Boolean!, $limit: Int!, $codigoSucursal: Int!, $codigoPuntoVenta: Int!) {
                    facturaCompraVentaAll(
                        reverse: $reverse
                        limit: $limit
                        entidad: { codigoSucursal: $codigoSucursal, codigoPuntoVenta: $codigoPuntoVenta }
                    ) {
                        docs {
							_id
							cafc
							cliente {
								_id
								apellidos
								codigoCliente
								codigoExcepcion
								complemento
								createdAt
								email
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
							codigoRecepcion
							createdAt
							cuf
							cufd {
								codigo
								codigoControl
								direccion
								fechaInicial
								fechaVigencia
							}
							cuis {
								codigo
								fechaVigencia
							}
							descuentoAdicional
							detalle {
								actividadEconomica {
									codigoCaeb
									descripcion
									tipoActividad
								}
								cantidad
								descripcion
								detalleExtra
								montoDescuento
								nroItem
								numeroImei
								numeroSerie
								precioUnitario
								producto
								productoServicio {
									codigoActividad
									codigoProducto
									descripcionProducto
								}
								subTotal
								unidadMedida {
									codigoClasificador
									descripcion
								}
							}
							detalleExtra
							documentoSector {
								codigoClasificador
								descripcion
							}
							eventoSignificativo
							fechaEmision
							leyenda
							metodoPago {
								codigoClasificador
								descripcion
							}
							moneda {
								codigoClasificador
								descripcion
							}
							montoGiftCard
							montoTotal
							montoTotalLiteral
							montoTotalMoneda
							montoTotalSujetoIva
							motivoAnulacion {
								codigoClasificador
								descripcion
							}
							nitEmisor
							numeroFactura
							numeroTarjeta
							puntoVenta {
								codigo
								descripcion
								nombre
								tipoPuntoVenta {
									codigoClasificador
									descripcion
								}
							}
							razonSocialEmisor
							representacionGrafica {
								pdf
								rollo
								sin
								xml
							}
							state
							subLeyenda
							sucursal {
								codigo
								departamento {
									codigo
									codigoPais
									departamento
									sigla
								}
								direccion
								municipio
								telefono
							}
							tipoCambio
							tipoEmision {
								codigoClasificador
								descripcion
							}
							tipoFactura {
								codigoClasificador
								descripcion
							}
							updatedAt
							usuario
							usucre
							usumod
						}
					}
				}",
            Variables = new
            {
                reverse,
                limit,
                codigoSucursal,
                codigoPuntoVenta
            }
        };

        var response = await _graphQLClient.SendQueryAsync<dynamic>(request);

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
            return response.Data;
        }
    }
}

class queryFacturas
{
    public static async Task ConsultarFacturas(bool reverse, int limit, int codigoSucursal, int codigoPuntoVenta)
    {
        // Suponiendo que tienes la URL y el token almacenados en variables globales
        var graphqlUrl = GlobalVariables.GraphQLUrl;
        var token = GlobalVariables.Token;

        var queryService = new GraphQLQueryService(graphqlUrl, token);
        var facturaCompraVentaData = await queryService.QueryFacturaCompraVentaAll(reverse, limit, codigoSucursal, codigoPuntoVenta);

        if (facturaCompraVentaData != null)
        {
            // Procesar los datos de la facturaCompraVentaData aquí
            Console.WriteLine(facturaCompraVentaData);
        }
        else
        {
            Console.WriteLine("Error en la consulta GraphQL.");
        }
    }
}
