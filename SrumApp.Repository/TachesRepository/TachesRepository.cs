using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ScrumApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SrumApp.Repository
{
    public class TachesRepository : BaseRepository
    {
        private readonly IConfiguration _configuration;

        public TachesRepository(IConfiguration configuration): base(configuration)
        {
            _configuration = configuration;
        }

        public List<TacheModel> GetTachesForProjet(int idProjet)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                select 
                t.idtaches, 
                t.nom,
                t.description,
                t.status
                from taches t
                where
                t.id_projet_parent = @idProjet
            ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@idProjet", idProjet);

            var reader = cmd.ExecuteReader();

            List<TacheModel> result = new List<TacheModel>();

            while (reader.Read())
            {
                result.Add(new TacheModel()
                {
                    Id = Convert.ToInt16(reader["idtaches"]),
                    Nom = reader["nom"].ToString(),
                    Description = reader["description"].ToString(),
                    Status = (StatusTache)Convert.ToInt16(reader["status"])
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

            cnn.Close();
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
            cnn.Close();
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
            cnn.Close();
        }

        public void EditProject(ProjectModel projectModel)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                UPDATE projet SET
                    nom = @nom,
                    description = @description,
                    date_debut = @date_debut,
                    date_fin = @date_fin,
                    id_proprietaire = @id_proprietaire
                WHERE 
                    idprojet = @idprojet
                ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@nom", projectModel.Name);
            cmd.Parameters.AddWithValue("@description", projectModel.Description);
            cmd.Parameters.AddWithValue("@date_debut", projectModel.DateDebut);
            cmd.Parameters.AddWithValue("@date_fin", projectModel.DateFin);
            cmd.Parameters.AddWithValue("@id_proprietaire", projectModel.Proprietaire.IdUser);
            cmd.Parameters.AddWithValue("@idprojet", projectModel.IdProjet);

            var isOK = cmd.ExecuteNonQuery();

            cnn.Close();
        }

        public void AddTache(TacheModel tacheModel)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                INSERT INTO taches
                    (
                    nom,
                    description,
                    status,
                    id_projet_parent)
                    VALUES
                    (
                    @nom,
                    @description,
                    @status,
                    @id_projet_parent)
                ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@nom", tacheModel.Nom);
            cmd.Parameters.AddWithValue("@description", tacheModel.Description);
            cmd.Parameters.AddWithValue("@status", (int)tacheModel.Status);
            cmd.Parameters.AddWithValue("@id_projet_parent", tacheModel.IdProjet);

            cmd.ExecuteNonQuery();
            cnn.Close();
        }

        public void ChangeStatusTache(int idTache, StatusTache status)
        {
            var cnn = this.OpenConnexion();

            string sql = @"
                UPDATE taches SET
                    status = @status
                    WHERE idtaches = @idtaches
                ";

            var cmd = new MySqlCommand(sql, cnn);
            cmd.Parameters.AddWithValue("@status", (int)status);
            cmd.Parameters.AddWithValue("@idtaches", idTache);

            cmd.ExecuteNonQuery();
            cnn.Close();
        }

    }
}
