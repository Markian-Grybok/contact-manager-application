using System;
using System.Collections.Generic;
using System.Text;

namespace ContactManager.BLL.Models;

public class ContactCreateViewModel
{
    public string Name { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public bool Married { get; set; }
    public string Phone { get; set; }
    public decimal Salary { get; set; }
}
