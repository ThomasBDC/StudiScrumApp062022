using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ScrumApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SrumApp.Repository
{
    public class ProjectRepository : BaseRepository
    {
        private readonly IConfiguration _configuration;

        public ProjectRepository(IConfiguration configuration): base(configuration)
        {
            _configuration = configuration;
        }

        public List<ProjectModel> GetProjects(int idProprietaire)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                SELECT 
                    p.idprojet,
                    p.nom,
                    p.description,
                    p.date_debut,
                    p.date_fin,
                    u.forename,
                    u.surname,
                    u.iduser
                FROM projet p
                INNER JOIN 
                    users u on u.iduser = p.id_proprietaire
                WHERE 
                    p.id_proprietaire = @idProprietaire
            ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@idProprietaire", idProprietaire);

            var reader = cmd.ExecuteReader();

            List<ProjectModel> result = new List<ProjectModel>();

            while (reader.Read())
            {
                result.Add(new ProjectModel()
                {
                    IdProjet = Convert.ToInt16(reader["idprojet"]),
                    Name = reader["nom"].ToString(),
                    Description = reader["description"].ToString(),
                    DateDebut = DateTime.Parse(reader["date_debut"].ToString()),
                    DateFin = DateTime.Parse(reader["date_fin"].ToString()),
                    Proprietaire = new UserModel()
                    {
                        IdUser = Convert.ToInt16(reader["iduser"]),
                        Forename = reader["forename"].ToString(),
                        Surname = reader["surname"].ToString(),
                    }
                });
            }

            return result;
        }


        public ProjectModel GetProject(int idProjet)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                SELECT 
                    p.idprojet,
                    p.nom,
                    p.description,
                    p.date_debut,
                    p.date_fin,
                    u.forename,
                    u.surname,
                    u.iduser
                FROM projet p
                INNER JOIN 
                    users u on u.iduser = p.id_proprietaire
                WHERE 
                    p.idprojet = @idProjet
            ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@idProjet", idProjet);

            var reader = cmd.ExecuteReader();

            ProjectModel result = null;

            if (reader.Read())
            {
                result = new ProjectModel()
                {
                    IdProjet = Convert.ToInt16(reader["idprojet"]),
                    Name = reader["nom"].ToString(),
                    Description = reader["description"].ToString(),
                    DateDebut = DateTime.Parse(reader["date_debut"].ToString()),
                    DateFin = DateTime.Parse(reader["date_fin"].ToString()),
                    Proprietaire = new UserModel()
                    {
                        IdUser = Convert.ToInt16(reader["iduser"]),
                        Forename = reader["forename"].ToString(),
                        Surname = reader["surname"].ToString(),
                    }
                };
            }

            return result;
        }

        public void AddProject(ProjectModel projectModel)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                INSERT INTO projet
                    (
                    nom,
                    description,
                    date_debut,
                    date_fin,
                    id_proprietaire)
                    VALUES
                    (
                    @nom,
                    @description,
                    @date_debut,
                    @date_fin,
                    @id_proprietaire);

                ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@nom", projectModel.Name);
            cmd.Parameters.AddWithValue("@description", projectModel.Description);
            cmd.Parameters.AddWithValue("@date_debut", projectModel.DateDebut);
            cmd.Parameters.AddWithValue("@date_fin", projectModel.DateFin);
            cmd.Parameters.AddWithValue("@id_proprietaire", projectModel.Proprietaire.IdUser);

            cmd.ExecuteNonQuery();
        }


        public void DeleteProject(int idProjet)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                DELETE FROM projet
                where idprojet = @idprojet
            ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@idprojet", idProjet);

            cmd.ExecuteNonQuery();
        }
    }
}
