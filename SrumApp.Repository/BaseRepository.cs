using Microsoft.Extensions.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;

namespace SrumApp.Repository
{
    public class BaseRepository
    {
        public string ConectionString { get; set; }

        public BaseRepository(IConfiguration configuration)
        {
            var builder = new MySqlConnectionStringBuilder();
            builder.Server = configuration["DbServer"];
            builder.Database = configuration["DbDatabase"];
            builder.UserID = configuration["DbUid"];
            builder.Password = configuration["DbPassword"];
            ConectionString = builder.ConnectionString + ";";
        }

        public MySqlConnection OpenConnexion()
        {
            try
            {
                MySqlConnection cnn = new MySqlConnection(ConectionString);
                cnn.Open();
                return cnn;
            }
            catch(Exception ex)
            {
                throw new Exception("Impossible de se connecter à la base de données");
            }
        }

    }
}
