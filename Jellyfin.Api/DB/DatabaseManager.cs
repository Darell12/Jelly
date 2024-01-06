using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Jellyfin.Api.DB
{
    internal class DatabaseManager
    {
        private readonly string _connectionString;

        #pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public DatabaseManager()
        #pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var databaseSettings = new DatabaseSettings();
            configuration.GetSection("DatabaseSettingsTenancies").Bind(databaseSettings);

        #pragma warning disable CS8601 // Posible asignación de referencia nula
            _connectionString = databaseSettings.ConnectionString;
        #pragma warning restore CS8601 // Posible asignación de referencia nula
        }

        public async Task<List<string>> ExecuteQueryAsync(string query)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                var result = new List<string>();

                #pragma warning disable CA2100 // Revisar consultas SQL para comprobar si tienen vulnerabilidades de seguridad
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(true))
                    {
                        result.Add(reader.GetString(0)); // Cambia esto según la estructura de tus datos
                    }
                }
                #pragma warning restore CA2100 // Revisar consultas SQL para comprobar si tienen vulnerabilidades de seguridad

                await connection.CloseAsync().ConfigureAwait(false);

                return result;
            }
        }
    }
}
