using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using ScrumApp.Models;

namespace SrumApp.Repository.UserRepository
{
    public class AuthRepository : BaseRepository
    {
        private readonly IConfiguration _configuration;

        public AuthRepository(IConfiguration configuration): base(configuration)
        {
            _configuration = configuration;
        }

        public void SignUp(UserModel userModel, string passwordSalt, string passwordHashed)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                INSERT INTO users
                (
                surname,
                forename,
                mail,
                password,
                phone,
                date_embauche,
                password_key)
                VALUES
                (
                @surname,
                @forename,
                @mail,
                @password,
                @phone,
                @date_embauche,
                @password_key);
                ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@surname", userModel.Surname);
            cmd.Parameters.AddWithValue("@forename", userModel.Forename);
            cmd.Parameters.AddWithValue("@mail", userModel.Mail);
            cmd.Parameters.AddWithValue("@password", passwordHashed);
            cmd.Parameters.AddWithValue("@phone", userModel.Phone);
            cmd.Parameters.AddWithValue("@date_embauche", userModel.DateEmbauche);
            cmd.Parameters.AddWithValue("@password_key", passwordSalt);

            cmd.ExecuteNonQuery();

            cnn.Close();
        }


        public void SetCleRecuperationForUser(int userId, string cleRecuperation)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                UPDATE users
                SET
                    cle_recuperation = @cleRecup,
                    date_clerecup = @datecleRecup
                WHERE
                    iduser = @iduser
                ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@cleRecup", cleRecuperation);
            cmd.Parameters.AddWithValue("@datecleRecup", DateTime.Now);
            cmd.Parameters.AddWithValue("@iduser", userId);

            cmd.ExecuteNonQuery();
            cnn.Close();
        }
    }
}
