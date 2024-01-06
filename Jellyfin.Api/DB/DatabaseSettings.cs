using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Jellyfin.Api.DB
{
    /// <summary>
    /// Settings de la database.
    /// </summary>
    public class DatabaseSettings
    {
        #pragma warning disable CS1591 // Falta el comentario XML para el tipo o miembro visible públicamente
        public string? ConnectionString { get; set; }
        #pragma warning restore CS1591 // Falta el comentario XML para el tipo o miembro visible públicamente
    }
}
