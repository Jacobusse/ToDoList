using ToDoList.Models;

public class Filters
{
    public string FilterString { get; }
    public Category Category { get; }
    public DueType Due { get; }
    public Status Status { get; }

    public bool HasCategory => Category != Category.All;
    public bool HasDue => Due != DueType.All;
    public bool HasStatus => Status != Status.All;
    public bool IsPast => Due == DueType.Past;
    public bool IsFuture => Due == DueType.Future;
    public bool IsToday => Due == DueType.Today;

    public Filters(string filter)
    {
        try
        {
            FilterString = filter;
            string[] filters = filter.Split('-');

            Category = Enum.Parse<Category>(filters[0]);
            Due = Enum.Parse<DueType>(filters[1]);
            Status = Enum.Parse<Status>(filters[2]);
        }
        catch { }

    }
}