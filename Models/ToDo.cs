using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ToDoList.Models;

public class ToDo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter a description")]
    public string Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "Please enter a due date")]
    public DateTime? DueDate { get; set; }

    [Required(ErrorMessage = "Please enter a Categroy")]
    public char CategoryId { get; set; } = (char)Category.Contact;
    [ValidateNever]
    public Category Category => (Category)CategoryId;

    [Required(ErrorMessage = "Please enter a Status")]
    public char StatusId { get; set; } = (char)Status.Open;
    [ValidateNever]
    public Status Status => (Status)StatusId;

    public bool Overdue => Status == Status.Open && DueDate < DateTime.Today;
}