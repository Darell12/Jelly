using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jellyfin.Api.Constants;
using Jellyfin.Api.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Api.Middleware;

/// <summary>
/// Middleware para interceptar las solicitudes antes de llegar al controlador capturando el jwt.
/// </summary>
public partial class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secrectKey =
        "ddee6a2109c0e7b03aefc530f1d4b3da6825515bf69e322a14cfaa7e0879e9e4";

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtMiddleware"/> class.
    /// </summary>
    /// <param name="next">El próximo delegado en la cadena de procesamiento.</param>
    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Método invocado al procesar una solicitud.
    /// </summary>
    /// <param name="context">El contexto de la solicitud HTTP.</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Lógica para leer el token JWT de la URL
        var token = context.Request.Query["token"].FirstOrDefault();
        if (context.Request.Path == "/videos/get" || context.Request.Path.StartsWithSegments("/videos", StringComparison.CurrentCultureIgnoreCase))
        {
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token no detectado.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized").ConfigureAwait(false);
                return;
            }

            if (!IsValidToken(token))
            {
                Console.WriteLine("Token invalido.");
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Invalid token").ConfigureAwait(false);
                return;
            }

            string referer = context.Request.Headers.Referer.ToString();
            Console.WriteLine(referer);
            if (!ValidarDominio(referer))
            {
                Console.WriteLine("El referer no coincide con al menos un dominio en la base de datos.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Dominio no autorizado").ConfigureAwait(false);
                return;
            }

            try
            {
                context.Items["info"] = GetInfoFromToken(token);
            }
            catch (SecurityTokenException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token").ConfigureAwait(false);
                return;
            }
        }

        // Continuar con el siguiente middleware
        await _next.Invoke(context).ConfigureAwait(false);
    }

    private static bool ValidarDominio(string referer)
    {
        var allValidDomains = new List<string>();
        const string queryDomain = "SELECT description FROM tenancy_domains";

        Database.CreateConnection(DBConnections.MangusProd);

        var validDomainsMangus = Database.Instance.PerformDatabaseOperation(queryDomain);
        allValidDomains.AddRange(validDomainsMangus);

        Database.CreateConnection(DBConnections.Tenancies);
        var validDomainsTenencias = Database.Instance.PerformDatabaseOperation(queryDomain);
        allValidDomains.AddRange(validDomainsTenencias);

        Database.Instance.CloseConnection();

        // Normalizar el referer y la lista de dominios
        string refererNormalizado = NormalizarReferer(referer);
        string[] dominiosNormalizados = allValidDomains.Select(NormalizeDomain).ToArray();

        bool coincide = dominiosNormalizados.Any(refererNormalizado.Contains);

        return coincide;
    }

    private static string NormalizarReferer(string referer)
    {
        // Normalizar la cadena, quitar "http://" y "https://", y convertir a minúsculas
#pragma warning disable CA1307 // Specify StringComparison for clarity

        return referer.Replace("http://", string.Empty).Replace("https://", string.Empty).Replace("/", string.Empty).ToLower(System.Globalization.CultureInfo.CurrentCulture);

#pragma warning restore CA1307 // Specify StringComparison for clarity
    }

    // Función para normalizar un dominio
    private static string NormalizeDomain(string domain)
    {
        // Convertir a minúsculas
        return domain.ToLower(System.Globalization.CultureInfo.CurrentCulture);
    }

    private bool IsValidToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.UTF8.GetBytes(_secrectKey);

            // Configuración de la validación del token
            #pragma warning disable CA5404 // No deshabilitar comprobaciones de validación de tokens
            var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            #pragma warning restore CA5404 // No deshabilitar comprobaciones de validación de tokens

            // Validar el token
            var principal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out var validatedToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private JObject? GetInfoFromToken(string token)
    {
        try
        {
            // Configuración del lector de token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.UTF8.GetBytes(_secrectKey); // Asegúrate de tener la clave correcta
            #pragma warning disable CA5404 // No deshabilitar comprobaciones de validación de tokens
            var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            #pragma warning restore CA5404 // No deshabilitar comprobaciones de validación de tokens

            var claimsPrincipal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out var validatedToken);

            if (claimsPrincipal?.Identity == null || ((ClaimsIdentity)claimsPrincipal.Identity).FindFirst("info") == null)
            {
                return null;
            }

            var infoClaim = ((ClaimsIdentity)claimsPrincipal.Identity).FindFirst("info");

            var jsonValue = JObject.Parse(infoClaim?.Value!);
            return jsonValue;
        }
        catch (Exception ex)
        {
            // Manejar cualquier excepción durante la decodificación del token
            Console.WriteLine($"Error al decodificar el token: {ex.Message}");
            return null;
        }
    }
}
