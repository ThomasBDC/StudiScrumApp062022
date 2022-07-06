using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScrumApp.Models;
using SrumApp.Repository;
using StudiScrumApp062022.Models;
using StudiScrumApp062022.Utils;
using System;
using System.Linq;
using System.Security.Claims;

namespace StudiScrumApp062022.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ProjectRepository _projectRepository;
        private readonly TachesRepository _tacheRepository;
        public ProjectController(ProjectRepository projectRepository, TachesRepository tacheRepository)
        {
            _projectRepository = projectRepository;
            _tacheRepository = tacheRepository;
        }

        // GET: ProjectController
        public ActionResult Index()
        {
            //Je récupère l'id de l'user connecté dans ses claims
            var idProprietaire = this.GetIdUserConnecte();

            //Je récupère les projets de l'utilisateur connecté
            var allProjects = _projectRepository.GetProjects(Convert.ToInt16(idProprietaire));

            return View(allProjects);
        }

        // GET: ProjectController/Details/5
        public ActionResult Details(int id)
        {
            var projet = _projectRepository.GetProject(id);
            var alltaches = _tacheRepository.GetTachesForProjet(id);
            var vm = new ProjetViewModel()
            {
                Projet = projet,
                allTaches = alltaches
            };
            return View(vm);
        }

        // GET: ProjectController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProjectController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProjectModel model)
        {
            try
            {
                var idProprietaire = this.GetIdUserConnecte();

                model.Proprietaire = new UserModel()
                {
                    IdUser = idProprietaire
                };

                //Faire la création en BDD
                _projectRepository.AddProject(model);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }

        // GET: ProjectController/Edit/5
        public ActionResult Edit(int id)
        {
            var monProjet = _projectRepository.GetProject(id);
            return View(monProjet);
        }

        // POST: ProjectController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ProjectModel model)
        {
            try
            {
                model.Proprietaire = new UserModel()
                {
                    IdUser = this.GetIdUserConnecte()
                };
                _projectRepository.EditProject(model);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }

        // GET: ProjectController/Delete/5
        public ActionResult Delete(int id)
        {
            var monProjet = _projectRepository.GetProject(id);
            return View(monProjet);
        }

        // POST: ProjectController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ProjectModel model)
        {
            try
            {
                _projectRepository.DeleteProject(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}
