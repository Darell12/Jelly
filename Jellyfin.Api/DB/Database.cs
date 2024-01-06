using System;
using System.Collections.Generic;
using Npgsql;

namespace Jellyfin.Api.DB
{
    /// <summary>
    /// Clase singleton para conexiones a base de datos.
    /// </summary>
    public class Database : IDisposable
    {
        private static Database? _instance;
        private readonly NpgsqlConnection _connection;

        private static readonly object lockObject = new object();

        /// <summary>
        /// Gets Instancia de conexion a base de datos.
        /// </summary>
        public static Database Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                        {
                            throw new InvalidOperationException("Se debe proporcionar una cadena de conexión al instanciar DatabaseConnection.");
                        }
                    }
                }

                return _instance;
            }
        }

        #pragma warning disable SA1201 // Elements should appear in the correct order
        private Database(string connectionString)
        #pragma warning restore SA1201 // Elements should appear in the correct order
        {
            _connection = new NpgsqlConnection(connectionString);
            try
            {
                _connection.Open();
                Console.WriteLine("Conexión a PostgreSQL establecida.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar a PostgreSQL: " + ex.Message);
            }
        }

        /// <summary>
        /// Metodo para ejecutar una query a la base de datos.
        /// </summary>
        /// <param name="query">String con formato de consulta SQL.</param>
        /// <returns>Retorna una lista de resultados.</returns>
        public ICollection<string> PerformDatabaseOperation(string query)
        {
            List<string> results = new List<string>();
            try
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                {
                    _connection.Open();
                }

                using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
                {
                    // Aquí puedes ejecutar tus operaciones en la base de datos utilizando NpgsqlCommand
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción según sea necesario
                Console.WriteLine($"Error en la operación de la base de datos: {ex.Message}");
            }
            finally
            {
                // Asegúrate de cerrar la conexión si es necesario
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return results;
        }

        /// <summary>
        /// Metodo para cerrar la conexion con la base de datos.
        /// </summary>
        public void CloseConnection()
        {
                _connection.Close();
                _connection.Dispose();
        }

        /// <summary>
        /// Método estático para inicializar la instancia con una cadena de conexión.
        /// </summary>
        /// <param name="connectionString">String que contiene conexion a la base de datos.</param>
        public static void Initialize(string connectionString)
        {
            lock (lockObject)
            {
               if (_instance == null)
               {
                    _instance = new Database(connectionString);
               }
            }
        }

        /// <summary>
        /// Metodo para descartar una instancia.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// No sé que poner aquí.
        /// </summary>
        /// <param name="disposing">Un Boolean.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberar recursos administrados
                if (_connection != null)
                {
                    _connection.Dispose();
                }
            }

            // Liberar recursos no administrados
        }

        /// <summary>
        /// Método para crear una conexión con una cadena de conexión específica.
        /// </summary>
        /// <param name="connectionString">te.</param>
        /// <exception cref="InvalidOperationException">tet.</exception>
        /// <returns>instancia.</returns>
        public static Database CreateConnection(string connectionString)
        {
            lock (lockObject)
            {
                if (_instance != null)
                {
                    // Puedes lanzar una excepción aquí o manejarlo según tus necesidades.
                    _instance.CloseConnection();
                    _instance.Dispose();
                }

                _instance = new Database(connectionString);
                return _instance;
            }
        }
    }
}
