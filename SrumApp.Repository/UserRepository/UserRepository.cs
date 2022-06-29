using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ScrumApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SrumApp.Repository.UserRepository
{
    public class UserRepository : BaseRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration): base(configuration)
        {
            _configuration = configuration;
        }

        public UserModel GetUser(string mail)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                SELECT 
                    u.iduser,
                    u.surname,
                    u.forename,
                    u.mail,
                    u.password,
                    u.phone,
                    u.date_embauche,
                    u.date_renvoi,
                    u.subjectid,
                    u.password_key
                FROM users u
                where u.mail = @mail;
            ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@mail", mail);

            var reader = cmd.ExecuteReader();

            UserModel user = null;

            if (reader.Read())
            {
                user = new UserModel()
                {
                    IdUser = Convert.ToInt16(reader["iduser"]),
                    Surname = reader["surname"].ToString(),
                    Forename = reader["forename"].ToString(),
                    Mail = reader["mail"].ToString(),
                    Password = reader["password"].ToString(),
                    Phone = reader["phone"].ToString(),
                    PasswordKey = reader["password_key"].ToString(),
                };
            }

            return user;
        }
    }
}
