using Microsoft.AspNetCore.Mvc;
using Research_Repository.Data;
using Research_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Research_Repository.Controllers
{
    public class ThemeController : Controller
    {

        private readonly ApplicationDbContext _db;

        public ThemeController(ApplicationDbContext db)
        {
            _db = db;
        }

    public IActionResult Index()
        {
        IEnumerable<Theme> objList = _db.Theme;
            return View(objList);
        }
    }
}
