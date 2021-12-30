using System;
using System.ComponentModel.DataAnnotations;

namespace EF.GeneratedSelectBug.Models;

public class Project
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; }
}