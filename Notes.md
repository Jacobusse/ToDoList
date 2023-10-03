- Want to change strings into Enums...
  - change class to enum.
  - copy out the values.
```csharp

ViewBag.Categories = Enum.GetValues<Category>();
```
  - Use Route to load Enums.