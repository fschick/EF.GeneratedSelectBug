using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EF.GeneratedSelectBug.Models;

public class Customer
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public List<Project> Projects { get; set; }

    public List<Order> Orders { get; set; }
}