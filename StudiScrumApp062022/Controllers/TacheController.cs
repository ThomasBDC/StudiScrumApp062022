using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScrumApp.Models;
using SrumApp.Repository;

namespace StudiScrumApp062022.Controllers
{
    public class TacheController : Controller
    {
        private readonly TachesRepository _tachesRepository;

        public TacheController(TachesRepository tachesRepository)
        {
            _tachesRepository = tachesRepository;
        }



        // GET: TacheController/Create
        public ActionResult Create(int idProjet)
        {
            var vm = new TacheModel()
            {
                IdProjet = idProjet
            };
            return View(vm);
        }

        // POST: TacheController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TacheModel input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(input);
                }

                //Faire la modification en BDD
                _tachesRepository.AddTache(input);

                return RedirectToAction("Details", "Project", new { id = input.IdProjet });
            }
            catch
            {
                return View();
            }
        }

        // GET: TacheController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TacheController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction("Index", "Project");
            }
            catch
            {
                return View();
            }
        }

        // GET: TacheController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TacheController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction("Index", "Project");
            }
            catch
            {
                return View();
            }
        }
    }
}
