namespace Jellyfin.Api.Constants
{
    /// <summary>
    /// Databases Configurations.
    /// </summary>
    public static class DBConnections
    {
        /// <summary>
        /// ConnectionString a Mangus Productivo.
        /// </summary>
        public const string MangusProd = "server=mangus-prod.cskfoquuftxn.us-east-1.rds.amazonaws.com;port=5432;database=mangus;username=dev_user;password=XfRAQ.gO%a$Snj2";

        /// <summary>
        /// ConnectionString a Tenencias.
        /// </summary>
        public const string Tenancies = "server=tenancies.cskfoquuftxn.us-east-1.rds.amazonaws.com;port=5432;database=tenancies;username=dev_user;password=XfRAQ.gO%a$Snj2";

        /// <summary>
        /// ConnectionString a Mangus Develop.
        /// </summary>
        public const string MangusDev = "server=tenancies.cskfoquuftxn.us-east-1.rds.amazonaws.com;port=5432;database=mangusdev;username=dev_user;password=XfRAQ.gO%a$Snj2";
    }
}
