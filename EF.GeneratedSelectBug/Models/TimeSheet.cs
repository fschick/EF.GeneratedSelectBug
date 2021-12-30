using System;
using System.ComponentModel.DataAnnotations;

namespace EF.GeneratedSelectBug.Models;

public class TimeSheet
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    public Project Project { get; set; }

    public Guid? OrderId { get; set; }

    public Order Order { get; set; }
}