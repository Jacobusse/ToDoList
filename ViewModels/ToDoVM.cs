using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoList.Models;

namespace ToDoList.ViewModels;

public class ToDoVM
{

    public List<ToDo> ToDos { get; set; }

    public Filters Filters { get; set; }

    public List<SelectListItem> Categories { get; init; }
    public IEnumerable<SelectListItem> DueTypes { get; init; }
    public IEnumerable<SelectListItem> Statuses { get; init; }

    public ToDoVM()
    {
        Categories = Enum.GetValues<Category>().Select(x => new SelectListItem(x.ToString(), x.ToString())).ToList();
        DueTypes = Enum.GetValues<DueType>().Select(x => new SelectListItem(x.ToString(), x.ToString())).ToList();
        Statuses = Enum.GetValues<Status>().Select(x => new SelectListItem(x.ToString(), x.ToString())).ToList();
    }
}
