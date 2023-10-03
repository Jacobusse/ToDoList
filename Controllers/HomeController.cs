using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;
using ToDoList.ViewModels;

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
        ToDoVM todo = new ToDoVM() { Filters = filters };

        IQueryable<ToDo> query = _db.ToDos;

        if (filters.HasCategory)
        {
            query = query.Where(t => t.CategoryId == (char)filters.Category);
        }
        if (filters.HasStatus)
        {
            query = query.Where(t => t.StatusId == (char)filters.Status);
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

        todo.ToDos = query.OrderBy(t => t.DueDate).ToList();
        return View(todo);
    }

    [HttpPost]
    public IActionResult Filter(string[] filters) 
    {
        string filter = string.Join("-", filters);
        return RedirectToAction("Index", new { filter });
    }

    [HttpPost]
    public IActionResult MarkComplete([FromRoute] string filter, ToDo selected)
    {
        selected = _db.ToDos.Find(selected.Id)!;
        if (selected != null)
        {
            selected.StatusId = (char)Status.Done;
            _db.SaveChanges();
        }
        return RedirectToAction("Filter", new { filter });
    }

    [HttpGet]
    public IActionResult Add()
    {
        ViewBag.Categories = Enum.GetValues<Category>();
        ViewBag.Statuses = Enum.GetValues<Status>();
        var task = new ToDo { StatusId = (char)Status.Open };
        return View(task);
    }

    [HttpPost]
    public IActionResult Add(ToDo task)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = Enum.GetValues<Category>();
            ViewBag.Statuses = Enum.GetValues<Status>();
            return View(task);
        }

        _db.ToDos.Add(task);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeleteComplete(string filter)
    {
        var toDelete = _db.ToDos.Where(t => t.Status == Status.Done).ToList();
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
