namespace ContactManager.BLL.Models;

public class ContactResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public bool Married { get; set; }
    public string Phone { get; set; } = null!;
    public decimal Salary { get; set; }
}
