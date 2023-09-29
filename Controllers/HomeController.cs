using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ToDoContext _db;

    public HomeController(ToDoContext db, ILogger<HomeController> logger)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index(string filter)
    {
        var filters = new Filters(filter);
        ViewBag.Filters = filters;
        ViewBag.Categories = _db.Categories.ToList();
        ViewBag.Statuses = _db.Statuses.ToList();
        ViewBag.DueFilters = Filters.DueFilterValues;

        IQueryable<ToDo> query = _db.ToDos
          .Include(t => t.Category)
          .Include(t => t.Status);

        if (filters.HasCategory)
        {
            query = query.Where(t => t.CategoryId == filters.CategoryId);
        }
        if (filters.HasStatus)
        {
            query = query.Where(t => t.StatusId == filters.StatusId);
        }
        if (filters.HasDue)
        {
            var today = DateTime.Today;
            if (filters.IsPast)
                query = query.Where(t => t.DueDate < today);
            else if (filters.IsFuture)
                query = query.Where(t => t.DueDate > today);
            else if (filters.IsToday)
                query = query.Where(t => t.DueDate == today);
        }
        var tasks = query.OrderBy(t => t.DueDate).ToList();
        return View(tasks);
    }


    [HttpPost]
    public IActionResult Filter(string[] filter)
    {
        string filterstring = string.Join('-', filter);
        return RedirectToAction("Index", new { filter = filterstring });
    }

    [HttpPost]
    public IActionResult MarkComplete([FromRoute] string filter, ToDo selected)
    {
        selected = _db.ToDos.Find(selected.Id)!;
        if (selected != null)
        {
            selected.StatusId = "closed";
            _db.SaveChanges();
        }
        return RedirectToAction("Index", new { filter });
    }

    [HttpGet]
    public IActionResult Add()
    {
        ViewBag.Categories = _db.Categories.ToList();
        ViewBag.Statuses = _db.Statuses.ToList();
        var task = new ToDo { StatusId = "open" };
        return View(task);
    }

    [HttpPost]
    public IActionResult Add(ToDo task)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = _db.Categories.ToList();
            ViewBag.Statuses = _db.Statuses.ToList();
            return View(task);
        }

        _db.ToDos.Add(task);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeleteComplete(string filter)
    {
        var toDelete = _db.ToDos.Where(t => t.StatusId == "closed").ToList();
        foreach (var task in toDelete)
        {
            _db.ToDos.Remove(task);
        }
        _db.SaveChanges();
        return RedirectToAction("Index", new { filter });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
