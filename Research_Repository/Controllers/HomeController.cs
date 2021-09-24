using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_Models.ViewModels;
using System.Linq;

namespace Research_Repository.Controllers
{
    public class HomeController : Controller
    {
        private readonly IThemeRepository _themeRepo;
        private readonly ITeamRepository _teamRepo;

        public HomeController(IThemeRepository themeRepo, ITeamRepository teamRepo)
        {
            _themeRepo = themeRepo;
            _teamRepo = teamRepo;
        }

        public IActionResult Index()
        {

            HomeVM homeVM = new HomeVM
            {
                Themes = _themeRepo.GetAll().ToList(),

                Teams = _teamRepo.GetAll(include: source => source
                .Include(a => a.Projects)).ToList()
            };
            return View(homeVM);
        }

    }
}
